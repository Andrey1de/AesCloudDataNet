using AesCloudDataNet.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AesCloudDataNet.Exceptions.StoreException;

namespace AesCloudDataNet.Services
{


    public abstract class AbstractSpool<TKey, T> where T : class, new()
    {
        public class SpoolItem<TItemKey, TItem> where TItem : class, new()
        {
            public readonly TItemKey Key;
            public TItem Item;
            public DateTime Updated;
            public SpoolItem()
            {

            }
            public SpoolItem(TItemKey key, TItem item)
            {
                Key = key;
                Item = item;
                Updated = new DateTime();
            }

        }
        static AbstractSpool()
        {
            DictInternal = new ConcurrentDictionary<TKey, SpoolItem<TKey, T>>();
        }
        private static readonly
            ConcurrentDictionary<TKey, SpoolItem<TKey, T>> DictInternal;

        public AbstractSpool(int actual)
        {
            ActualForMs = actual;

        }
        public int ActualForMs { get; protected set; } = 1000 * 3600;
        public bool UseDB { get; private set; }



        public virtual bool HasItem(TKey key) => DictInternal.Keys.Contains(key);

        protected bool IsValid(SpoolItem<TKey, T> item)
        {
            return (ActualForMs <= 0) ||
            (item.Updated.AddMilliseconds(ActualForMs) > DateTime.Now);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T SpoolGet(TKey key)
        {
            SpoolItem<TKey, T> store = GetSpoolItem(key);
            return store?.Item;

        }
        protected SpoolItem<TKey, T> GetSpoolItem(TKey key)
        {
            SpoolItem<TKey, T> store = null;
            if (DictInternal.TryGetValue(key, out store))
            {
                if (!IsValid(store))
                {
                    SpoolItem<TKey, T> store1 = null;
                    DictInternal.TryRemove(key, out store1);
                }
            }

            return store;

        }


        public List<T> SpoolList()
        {
            return DictInternal.Values.Select(p => p.Item).ToList();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns>True if was Added / False Updated , and real Existing T</returns>
        public bool SpoolAddOrUpdate(TKey key, T item)
        {
            T itemRet;
            return SpoolAddOrUpdate(key, item, out itemRet);
        }
        public bool SpoolAddOrUpdate(TKey key, T item, out T itemRet)
        {
            bool added = true; itemRet = null;
            if (item == null)
            {
                SpoolItem<TKey, T> spi
                DictInternal.TryRemove(key, out spi);
                itemRet = null;
                return false;


            }
            itemRet = DictInternal.AddOrUpdate(key,
                key => new SpoolItem<TKey, T>(key, item),
                    (key, old) =>
                    {
                        added = false;
                        old.Item = item;
                        old.Updated = DateTime.Now;
                        return old;
                    }).Item;

            return added;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Old Item if was / null</returns>
        public T SpoolDelete(TKey key)
        {
            SpoolItem<TKey, T> outRet = null;

            return (DictInternal.TryRemove(key, out outRet)) ? outRet.Item : null;

        }

        public T SpoolClear()
        {
            DictInternal.Clear();
            return store?.Item;

        }

#endregion



    }
}
