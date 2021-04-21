using AesCloudDataNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AesCloudDataNet.Services
{
    public interface IExchangeRateService :
            IDalAbstractHttpService<string, RateToUsd>
    {
     
    }

    public class ExchangeRateService :
        DalAbstractHttpService<string, RateToUsd>,
        IExchangeRateService
    {
        private readonly HttpClient Client ;
        ILogger<ExchangeRateService> Log; 

        public ExchangeRateService(ILogger<ExchangeRateService> logger, HttpClient client)
            : base(logger, client)
        {
            Log = logger;
            Client = client;
        }
        /// <returns></returns>
        public async override Task<RateToUsd> Get(string key, bool toRetrieve)
        {
            return await base.Get(key,toRetrieve);

        }
 
        public async override Task<List<RateToUsd>> List(bool toRetrieve)
        {
            return await base.List(toRetrieve);
        }

        public async override Task<RateToUsd> Insert(string key, RateToUsd valueIn, bool toRetrieve)
        {
            return await base.Insert(key, valueIn, toRetrieve);
        }

        public async override Task<RateToUsd> Update(string key, RateToUsd valueIn, bool toRetrieve)
        {
            return await base.Update(key,valueIn ,toRetrieve);
        }

        public async override Task Delete(string key, bool toRetrieve)
        {
            await base.Delete(key, toRetrieve);
        }


        #region Storage Overridable don't use usePersist = true !!!!
        protected async override Task<RateToUsd> RetrieveStorageItem(string code)
        {
            throw new NotImplementedException();
        }


        protected override Task<Dictionary<string, RateToUsd>> RetrieveStorageItemsList()
        {
            throw new NotImplementedException();
        }


         #endregion
       
        readonly string[] AlphavantageSecretKeys = { "55Y1508W05UYQN3G", "3MEYVIGY6HV9QYMI" };

        public override string ConvertorUrl { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        //AND This Service Get form Http and stores to DB if neccessary
        protected async virtual Task<RateToUsd> GetItemByHttp(string code)
        {
            RateToUsd rate = null;
            try
            {
  
                string url =
                      "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&" +
                      "from_currency=USD&to_currency=" + code;

                string url0 = url + "&apikey=" + AlphavantageSecretKeys[0];
                string url1 = url + "&apikey=" + AlphavantageSecretKeys[1];
                rate = await ReceiveItem(code,url0);
                if (rate == null)
                {
                    rate = await ReceiveItem(code, url0);
                }

              
             }
            catch (Exception)
            {

                throw;
            }
            return rate;

        }


          private  async Task<RateToUsd> ReceiveItem(string code,string url)
        {
            var jsonBody = await HttpGetJson(Client, url);
           
            if (!jsonBody.Contains("1. From_Currency Code"))
                return null;

                string[] pars1 = new string[] {
                    "\"4. To_Currency Name\":",
                    "\"5. Exchange Rate\":",
                    "\"6. Last Refreshed\":",
                    "\"8. Bid Price\":" +
                    "\"9. Ask Price\":"};

            string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);

            Func<string, string> f1 = (str) =>
                str.Trim().Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];

            RateToUsd ret = new RateToUsd();
            ret.Code = code;
            ret.Name = f1(arrr[0]);
            ret.Rate = Double.Parse(arrr[1]);
            ret.LastRefreshed = DateTime.Parse(arrr[2]);
            ret.Ask = Double.Parse(arrr[3]);
            ret.Bid = Double.Parse(arrr[4]);
            ret.Stored = DateTime.Now;


            return ret;



        }

        public override RateToUsd DecodeBody(string code, string jsonBody)
        {
            throw new NotImplementedException();
        }

        public override Task<string> GetFromHttp(string key)
        {
            throw new NotImplementedException();
        }
    }
}
/*
        const ratesExchangeAxios = {
   "Realtime Currency Exchange Rate": {
       "1. From_Currency Code": "USD",
       "2. From_Currency Name": "United States Dollar",
       "3. To_Currency Code": "JPY",
       "4. To_Currency Name": "Japanese Yen",
       "5. Exchange Rate": "110.29100000",
       "6. Last Refreshed": "2021-04-06 08:16:01",
       "7. Time Zone": "UTC",
       "8. Bid Price": "110.29060000",
       "9. Ask Price": "110.29560000"
   }
}

        */