using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AesCloudData;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AesCloudDataNet.Controllers
{
    //public interface IDalAbstractServiceFacade<TKey, T> where T : class, new()
    //{
    //    public Task<T> Get(TKey key);
    //    public Task<List<T>> List();

    //    public Task<T> Insert(TKey key, T valueIn);

    //    public Task<T> Update(TKey key, T valueIn);
    //    public Task Delete(TKey key);

    //}
    public interface IDalAbstractService<TKey, T> where T : class, new()
    {
        public Task<T> Get(TKey key, bool usePersist);
        public Task<List<T>> List(bool usePersist);

        public Task<T> Insert(TKey key, T valueIn, bool usePersist);

        public Task<T> Update(TKey key, T valueIn, bool usePersist);
        public Task Delete(TKey key, bool usePersist);

    }
    public abstract class DalAbstractService<TKey, T> : IDalAbstractService<TKey, T>
        where T : class, new()
    { 
        protected class Store<T>
        {
            public readonly TKey Key;
            public T Item;
            public DateTime Updated;
            public Store(TKey key,T item)
            {
                Key = key;
                Item = item;
                Updated = new DateTime();
            }
            public bool Valid => (ActualForMs <= 0) ||
                Updated.AddMilliseconds(ActualForMs) > DateTime.Now;

        }

        public void Assert(object o)
        {
            if (o == null) throw new NullReferenceException();
        }
        protected static readonly ConcurrentDictionary<TKey, Store<T>> DictInternal;
        readonly private  ILogger Log;
        /// <summary>
        /// Stores how mach time in ms would be actual Item. Otherwise if 0 - imlimited
        /// </summary>
        public static int ActualForMs { get; set; } = 1000 * 3600;
        public Dictionary<TKey, T> Dict => DictInternal.ToDictionary(p => p.Key, p => p.Value.Item);
           // GET: api/<DalAbstractController>
        #region ctrors
        static DalAbstractService()
        {
            DictInternal = new ConcurrentDictionary<TKey, Store<T>>();
        }
        public DalAbstractService(ILogger _logger)
        {
            Log = _logger;
        }

        #endregion

        public virtual async Task<T> Get(TKey key, bool usePersist)
        {
            Store<T> store = InternalGet(key, usePersist);
           
            if (usePersist)
            {
                T item = await RetrieveStorageItem(key);

                if (item != default(T))
                {
                    DictInternal.TryAdd(key, new Store<T>(key, item));
                }

            }

            return store.Item;
        }
        public virtual async Task<List<T>> List(bool usePersist)
        {
            if (!usePersist)
            {
                return Dict.Values.ToList();
            }
            DictInternal.Clear();


            Dictionary<TKey, T> _dic = await RetrieveStorageItemsList();
            if (_dic != null)
            {
                foreach (var (key, value) in _dic)
                {
                    DictInternal.TryAdd(key, new Store<T>(key, value));
                }

            }
            return Dict.Values.ToList(); ;

        }
        /// <summary>
        /// Insert returns stored  item if this one doesn;t existed before, 
        /// Oothewise  - null
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueIn"></param>
        /// <param name="usePersist"></param>
        /// <returns></returns>
        public virtual async Task<T> Insert(TKey key, T valueIn, bool usePersist)
        {

            Store<T> store = InternalGet(key, usePersist);
            if (store != null )
            {
                return null; 
            }

            store = new Store<T>(key, valueIn);

            if (usePersist)
            {
                //May be DB Updates somteh
                try
                {
                    store.Item = await TryInserStoragetItem(key, valueIn);

                }
                catch (Exception)
                {
                    Log.LogWarning($"Conflict in Insert {key}");
                    DictInternal.TryRemove(key, out store);
                }


            } 
            if(store.Item != null)
            {
                DictInternal.TryAdd(key, store);

            }
            return store.Item;


        }
        /// <summary>
        /// Update value if this just exists
        /// In Hasn updated those Store value without Updating !!!
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueIn"></param>
        /// <param name="usePersist"></param>
        /// <returns></returns>
        public virtual async Task<T> Update(TKey key, T valueIn, bool usePersist)
        {
            Store<T> store = InternalGet(key, usePersist);
            //If value isn't exist return null
            if (store == null )
            {
                return null;
            }

            T item = valueIn;

            if (usePersist)
            {

                try
                {

                    item = await TryUpdateStorageItem(key, valueIn);

                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Conflict in update {key}");
                    DictInternal.TryRemove(key, out store);
                    return null;
                }

            }
            if(item != null)
            {
                store.Item = item;
                store.Updated = DateTime.Now;

            }
            return store.Item;



        }

        public virtual async Task Delete(TKey key, bool usePersist)
        {
            Store<T> store = null;
            DictInternal.TryRemove(key, out store);
            if (usePersist)
            {

                try
                {
                    await TryDeleteStorageItem(key);

                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Conflict in update {key}");
                    DictInternal.TryRemove(key, out store);
                } 
            }

        }

 
        private Store<T> InternalGet(TKey key, bool usePersist)
        {
            Store<T> store = null;
            if (DictInternal.TryGetValue(key, out store))
            {
                if (usePersist && !store.Valid)
                {
                    Store<T> val0 = null;
                    DictInternal.TryRemove(key, out val0);
                }
            }

            return store;

        }


  
        protected abstract Task<T> RetrieveStorageItem(TKey key);

        protected abstract Task<Dictionary<TKey, T>> RetrieveStorageItemsList();

  
        protected abstract Task<T> TryInserStoragetItem(TKey key, T valueIn);


        protected abstract Task<T> TryUpdateStorageItem(TKey key, T valueIn);

     
 
        protected abstract Task TryDeleteStorageItem(TKey key);

      

        
    }
}
