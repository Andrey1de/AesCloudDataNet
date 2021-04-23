using AesCloudDataNet.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AesCloudDataNet.Services.Abstract
{

    public interface IDalAbstractHttpService<TKey, T> where T : class, new()
        

    {
        public Task<T> Get(TKey key, bool useHttp);
        public List<T> List();
        public bool Delete(TKey key);

        public void AddRange(Dictionary<TKey,T> range );
        //  public string[] GetHttpUrls(TKey key);


    }
    public abstract class DalAbstractHttpService<TKey, T> :
            AbstractSpool<TKey, T>, IDalAbstractHttpService<TKey, T>
             where T : class, new()

    {
        private const bool USE_HTTP = true;
        private ILogger Log;
        protected HttpClient Client;

        // public abstract string ConvertorUrl { get; init; } //TBD from env
        // "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=";

        public DalAbstractHttpService(ILogger logger,
            HttpClient _httpClient, int actual = 3600 * 1000)
            : base(actual)

        {
            Client = _httpClient;
            Log = logger;
            PrepareHttpClient(Client);
        }

        /// <summary>
        /// Prepared for advantage company, for others = please override
        /// </summary>
        /// <param name="_httpClient"></param>
        /// <returns></returns>
        public virtual HttpClient PrepareHttpClient(HttpClient _httpClient)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/json,*/*");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "that2dollar");
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/html"));
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/text"));
            return _httpClient;
        }
        /// <summary>
        /// Decode body doesn't returns exceptions !!!
        /// </summary>
        /// <param name="code"></param>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        protected abstract T DecodeBody(TKey code, string jsonBody);


        protected abstract string[] GetHttpUrls(TKey key);



        #region Facade

        public virtual async Task<T> Get(TKey key, bool toRetrieve = USE_HTTP)
        {
            T item = base.SpoolGet(key);
            if (item != null && toRetrieve)
            {
                return item;
            }
            string[] urls = GetHttpUrls(key);
            foreach (var url in urls)
            {
                try
                {
                    string json = await Client.GetHttpStringAsync(url);
                    item = DecodeBody(key, json);

                    if (item != null)
                    {
                        SpoolAddOrUpdate(key, item);
                        return item;

                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"GetHttp for({key}) , \nUrl:{url}\n " + ex.StackTrace);
                }

            }

            return item;

        }


        public  virtual List<T> List()
        {
            return base.SpoolList();
        }

        public bool  Delete(TKey key)
        {
            bool b = base.SpoolDelete(key) != null;
            return b;
        }

        public void AddRange(Dictionary<TKey, T> range)
        {
            foreach (var item in range)
            {
                SpoolAddOrUpdate(item.Key, item.Value);
            }
        }


        #endregion




    }


}
