using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AesCloudDataNet.Services
{
    public abstract class DalAbstractHttpService222<TKey, T> :
        DalAbstractService<TKey, T>
         where T : class, new()

    {
        private ILogger Log;
        protected HttpClient Client;

        public abstract string ConvertorUrl { get; init; } //TBD from env
        // "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=";

        public DalAbstractHttpService(ILogger logger,
            HttpClient _httpClient)
            : base(logger)
           
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
        public abstract T DecodeBody(TKey code, string jsonBody);

        //{
        //    if (!jsonBody.Contains("1. From_Currency Code"))
        //    {
        //        Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
        //        return null;
        //    }
        //    string[] pars1 = new string[] {
        //            "\"3. To_Currency Code\":",
        //            "\"4. To_Currency Name\":",
        //            "\"5. Exchange Rate\":",
        //            "\"6. Last Refreshed\":",
        //            "\"8. Bid Price\":" ,
        //            "\"9. Ask Price\":"};

        //    string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);

        //    var code1 = f1(arrr[1]).ToUpper();
        //    if (arrr == null || arrr.Length < 7 || code1 != code)
        //    {
        //        Log.LogWarning($"DecodeBody : Error of Parsing= [{arrr}]");
        //        return null;
        //    }

        //    RateToUsd ret = new RateToUsd();
        //    ret.Code = code;


        //    if (string.IsNullOrWhiteSpace(ret.Code)
        //        || ret.Code != code)
        //    {
        //        return null;
        //    }

        //    ret.Name = f1(arrr[2]);
        //    ret.Rate = Double.Parse(f1(arrr[3]));
        //    ret.LastRefreshed = DateTime.Parse(f1(arrr[4]));
        //    ret.Ask = Double.Parse(f1(arrr[5]));
        //    ret.Bid = Double.Parse(f1(arrr[6]));
        //    ret.Stored = DateTime.Now;


        //    return ret;
        //}

        public  abstract Task<string> GetFromHttp(TKey key);
        //{

        //    try
        //    {
        //        string url = ConvertorUrl + code;

        //        string url0 = url + "&apikey=" + Secrets.ADVANTAGE_SECRET_0;



        //        RateToUsd body = await Spooler.GetHttpWithSpool(code, url0, MaxReadDelaySec, DecodeBody);

        //        if (body == null)
        //        {
        //            string url1 = url + "&apikey=" + Secrets.ADVANTAGE_SECRET_1;

        //            body = await Spooler.GetHttpWithSpool(code, url1, MaxReadDelaySec, DecodeBody);
        //        }

        //        return body;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogError(ex.Message + "\n" + ex.StackTrace);
        //        return null;
        //    }



        //}



        #region Facade
   
        public async override  Task<T> Get(TKey key, bool usePersist)
        {
            T item = await base.Get(key, usePersist);
            if (item == null)
            {
                string json  = await GetFromHttp(key);
                item = DecodeBody(key,json);
                if (item != null)
                {
                    item = await base.Insert(key, item, usePersist);

                }

            }
            return item;

        }

        #region Simple overrides may be not implemented
        public override bool HasItem(TKey key)
        {
            return base.HasItem(key);
        }

        public async override Task Delete(TKey key, bool usePersist)
        {
            await base.Delete(key, usePersist);
        }
        public async override Task<T> Insert(TKey key, T valueIn, bool usePersist)
        {
            return await base.Update(key, valueIn, usePersist);
        }

        public async override Task<List<T>> List(bool usePersist)
        {
            return await base.List(usePersist);
        }

        public async override Task<T> Update(TKey key, T valueIn, bool usePersist)
        {
            return await base.Update(key, valueIn, usePersist);
        }



        #endregion        
        #endregion
        
        #region Dummy stub overrides for use without db, if service doesn't use DB this is no important
        protected async override Task<Dictionary<TKey, T>> RetrieveStorageItemsList()
        {
            return null;
        }

        protected async override Task TryDeleteStorageItem(TKey key)
        {
            // throw new NotImplementedException();
        }

        protected async override Task<T> TryInserStoragetItem(TKey key, T valueIn)
        {
            return valueIn;// throw new NotImplementedException();
        }

        protected async override Task<T> TryUpdateStorageItem(TKey key, T valueIn)
        {
            return valueIn;
        } 
        #endregion
    }
}
