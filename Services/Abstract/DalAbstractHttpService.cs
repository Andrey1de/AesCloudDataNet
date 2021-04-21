using AesCloudDataNet.Exceptions;
using AesCloudDataNet.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AesCloudDataNet.Services.Abstract
{ 

    public interface IDalAbstractHttpService<TKey, T> where T : class, new()
        //IDalAbstractService<TKey, T> 

 {
    public Task<T> Get(TKey key, bool toRetrieve);
    public Task<List<T>> List(bool toRetrieve);
    public Task Delete(TKey key, bool toRetrieve);
  //  public string[] GetHttpUrls(TKey key);




}
public abstract class DalAbstractHttpService<TKey, T> :
        DalAbstractService<TKey, T>, IDalAbstractHttpService<TKey, T>
         where T : class, new()

{
    private const bool TO_RETRIEVE = true;
    private ILogger Log;
    protected HttpClient Client;

    // public abstract string ConvertorUrl { get; init; } //TBD from env
    // "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=";

    public DalAbstractHttpService(ILogger logger,
        HttpClient _httpClient, int actual = 3600 * 1000)
        : base(logger, actual)

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

    public async override Task<T> Get(TKey key, bool toRetrieve = TO_RETRIEVE)
    {
        T item = await base.Get(key, toRetrieve);
        if (item != null)
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

 
    public async override Task<List<T>> List(bool toRetrieve)
    {
        return await base.List(toRetrieve);
    }

    public async override Task Delete(TKey key, bool toRetrieve)
    {
        await base.Delete(key, toRetrieve);
    }


    #endregion


    #region Sealed Odd functions
    sealed public async override Task<T> Insert(TKey key, T valueIn, bool toRetrieve)
    {
        throw new NotImplementedException();
    }
    sealed public async override Task<T> Update(TKey key, T valueIn, bool toRetrieve)
    {
        throw new NotImplementedException();
    }


    #endregion


    #region Stub for unnecessary for http functions

    protected override Task<T> RetrieveStorageItem(TKey key) 
    {
        throw new NotImplementedException();
    }


    protected override Task<Dictionary<TKey, T>> RetrieveStorageItemsList()
    {
        throw new NotImplementedException();
    }

    protected override Task TryDeleteStorageItem(TKey key)
    {
        throw new NotImplementedException();
    }

    protected override Task<T> TryInserStoragetItem(TKey key, T valueIn)
    {
        throw new NotImplementedException();
    }

    protected override Task<T> TryUpdateStorageItem(TKey key, T valueIn)
    {
        throw new NotImplementedException();
    }


    #endregion


}
}
