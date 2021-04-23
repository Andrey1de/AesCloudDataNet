using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AesCloudDataNet.Exceptions;
using EType = AesCloudDataNet.Exceptions.StoreException.EType;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AesCloudDataNet.Services.Abstract
{
  
    public interface IDalAbstractService<TKey, T> where T : class, new()
    {
        public Task<T> Get(TKey key, bool toRetrieve);
        public Task<List<T>> List(bool toRetrieve);
        public Task<T> Insert(TKey key, T valueIn, bool toRetrieve);
        public Task<T> Update(TKey key, T valueIn, bool toRetrieve);
        public Task Delete(TKey key, bool toRetrieve);

        public bool HasItem(TKey key);

    }
    public abstract class DalAbstractService<TKey, T> : 
        AbstractSpool<TKey, T>, IDalAbstractService<TKey, T>
        where T : class, new()
    {
        /// <summary>
        /// Stores how mach time in ms would be actual Item. Otherwise if 0 - imlimited
        /// </summary>
       // public static  int ActualForMs { get; protected set; } = 1000 * 3600;

   

        readonly private  ILogger Log;
             // GET: api/<DalAbstractController>
        #region ctrors
        public DalAbstractService(ILogger _logger, int actual)
            : base(actual)
        {
            Log = _logger;
            ActualForMs = actual;
        }

        #endregion
        #region Facade Functions - IDalAbstractService<TKey, T>

        public virtual async Task<T> Get(TKey key, bool toRetrieve)//, bool useHttp)
        {
            T item = SpoolGet(key);
            if (item != null || !toRetrieve)
            {
                return item;
            }

            try
            {
                item = await RetrieveStorageItem(key);
                this.SpoolAddOrUpdate(key, item);

                return item;

            }
            catch (Exception ex)
            {
                string msg = $"Conflict in Get key={key} : {ex.Message}";
                ex = new StoreException(EType.Get, msg, ex);
                //   
                throw ex;
            }
        }


        public virtual async Task<List<T>> List(bool toRetrieve)
        {
            if (!toRetrieve)
            {
                return SpoolList();
            }
            SpoolClear();

            try
            {

                Dictionary<TKey, T> _dic = await RetrieveStorageItemsList();
                if (_dic != null)
                {
                    foreach (var (key, value) in _dic)
                    {
                        SpoolAddOrUpdate(key, value);
                    }

                }
                return SpoolList();

            }
            catch (Exception ex)
            {
                string msg = $"Conflict in List ";
                ex = new StoreException(EType.List, msg, ex);
                Log.LogError(ex.Message, ex);
                throw ex;
            }


        }
        /// <summary>
        /// Insert returns stored  item if this one doesn;t existed before, 
        /// Oothewise  - null
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueIn"></param>
        /// <param name="usePersist"></param>
        /// <returns></returns>
        public virtual async Task<T> Insert(TKey key, T valueIn, bool toRetrieve)
        {
            T item = null;
            if (!toRetrieve)
            {
                SpoolAddOrUpdate( key, valueIn, out item);
                return  item;

            }

            if (HasItem(key))
            {
                Exception ex = new StoreException(EType.Insert, $"The item with key={key} just present ");
                Log.LogError(ex.Message, ex);
                throw ex;

            }

     
            try
            {
                item = await TryInserStoragetItem(key, valueIn);
            }
            catch (Exception ex)
            {
                string msg = $"Insert Conflict key={key} ";
                ex = new StoreException(EType.Insert, $"Insert Conflict key={key} ", ex);
               // Log.LogError(ex.Message, ex);
                throw ex;
            }

            if (item == null)
            {
                string msg = $"Insert Conflict key={key} ";
                Exception ex = new StoreException(EType.Insert, $"Insert conflict, key={key}");
               // Log.LogError(ex.Message, ex);
                throw ex;

            }


            SpoolAddOrUpdate(key, item, out item);
            return item;


        }


        /// <summary>
        /// Update value if this just exists
        /// In Hasn updated those Store value without Updating !!!
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueIn"></param>
        /// <param name="usePersist"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(TKey key, T valueIn, bool toRetrieve)
        {

            T item = null;
            if (!toRetrieve)
            {
                SpoolAddOrUpdate(key, valueIn, out item);
                return item;
            }


             //If value isn't exist return null
            if (!HasItem(key))
            {
                Exception ex = new StoreException(EType.Update,
                                $"The item with key={key} not present");
                //Log.LogError(ex.Message, ex);
                throw ex;
            }


            try
            {
                item = await TryUpdateStorageItem(key, valueIn);
                
            }
            catch (Exception ex)
            {
               SpoolDelete(key);
                ex = new StoreException(EType.Update,
                    $"Update conflict key={key}", ex);
    //            Log.LogError(ex.Message, ex);
                throw ex;

            }
            if (item == null)
            {
                SpoolDelete(key);

                Exception ex = new StoreException(EType.Update,
                                 $"Update conflict key={key}");
     //           Log.LogError(ex.Message, ex);
                throw ex;
            }

            return item;
        }


        public virtual async Task Delete(TKey key, bool usePersist)
        {
             SpoolDelete(key);
            if (usePersist)
            {

                try
                {
                    await TryDeleteStorageItem(key);

                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Conflict in update {key} : {ex.Message}");
                   
                }
            }

        }

        #endregion

        #region Abstract Storage Functions
        protected abstract Task<T> RetrieveStorageItem(TKey key);

        protected abstract Task<Dictionary<TKey, T>> RetrieveStorageItemsList();


        protected abstract Task<T> TryInserStoragetItem(TKey key, T valueIn);


        protected abstract Task<T> TryUpdateStorageItem(TKey key, T valueIn);



        protected abstract Task TryDeleteStorageItem(TKey key);

        #endregion


    }
}
