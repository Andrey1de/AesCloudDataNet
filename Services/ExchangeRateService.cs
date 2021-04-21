using AesCloudDataNet.Models;
using AesCloudDataNet.Services.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

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
        const int ACTUAL_MS = 1000 * 3600;
       // private readonly HttpClient Client;
        ILogger<ExchangeRateService> Log;

        public ExchangeRateService(ILogger<ExchangeRateService> logger,
            HttpClient client)
            : base(logger, client, ACTUAL_MS)
        {
            Log = logger;
            Client = client;
        }
        //public override HttpClient PrepareHttpClient(HttpClient _httpClient)
        //{
        //    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/json,*/*");
        //    _httpClient.DefaultRequestHeaders.Add("User-Agent", "that2dollar");
        //    _httpClient.DefaultRequestHeaders.Accept
        //          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    _httpClient.DefaultRequestHeaders.Accept
        //          .Add(new MediaTypeWithQualityHeaderValue("application/html"));
        //    _httpClient.DefaultRequestHeaders.Accept
        //          .Add(new MediaTypeWithQualityHeaderValue("application/text"));
        //    return _httpClient;
        //}
        protected override string[] GetHttpUrls(string code)
        {
            string url = "https://www.alphavantage.co/query?" 
                       + "function=CURRENCY_EXCHANGE_RATE&" 
                       + "from_currency=USD&to_currency=" + code;
            List<String> list = new List<string>();

            list.Add(url + "&apikey=" + Secrets.ADVANTAGE_SECRET[0]);
            list.Add(url + "&apikey=" + Secrets.ADVANTAGE_SECRET[1]);
            return list.ToArray();
        }

        /*
        const ratesExchangeAxios = {
   'Realtime Currency Exchange Rate': {
       '1. From_Currency Code': 'USD',
       '2. From_Currency Name': 'United States Dollar',
       '3. To_Currency Code': 'JPY',
       '4. To_Currency Name': 'Japanese Yen',
       '5. Exchange Rate': '110.29100000',
       '6. Last Refreshed': '2021-04-06 08:16:01',
       '7. Time Zone': 'UTC',
       '8. Bid Price': '110.29060000',
       '9. Ask Price': '110.29560000'
   }
        */
        protected override RateToUsd DecodeBody(string code, string jsonBody)
        {
   

        JObject o1 = JObject.Parse(jsonBody);
            if (o1 == null)
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }

            JToken o = o1["Realtime Currency Exchange Rate"];
            Func<string, string> fs = (string name) => (string)o[name] ?? "";
            Func<string, int> fi = (string name) => int.Parse((string)o[name] ?? "0");
            Func<string, double> fd = (string name) => double.Parse((string)o[name] ?? "0.0");

            if (o == null)
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }

            //Func<string, string> fs = (string name) => (string)o[name] ?? "";
            //Func<string, int> fi = (string name) => int.Parse((string)o[name] ?? "0");
            //Func<string, double> fd = (string name) => double.Parse((string)o[name] ?? "0.0");

        
            var code1 = fs("3. To_Currency Code")?.ToUpper(); 
            if ( code1 != code)
            {
                Log.LogWarning($"DecodeBody : Error of Parsing= {{{jsonBody}}}");
                return null;
            }

            RateToUsd ret = new RateToUsd();
            ret.Code = code;


            ret.Name = fs("4. To_Currency Name");
            ret.Rate = fd("Exchange Rate");
            ret.LastRefreshed = DateTime.Parse(fs("6. Last Refreshed"));
            ret.Bid = fd("8. Bid Price");
            ret.Ask = fd("9. Ask Price");
            ret.Stored = DateTime.Now;


            return ret;
        }



    }

}
