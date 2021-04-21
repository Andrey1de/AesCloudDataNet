using AesCloudDataNet.Models;
using AesCloudDataNet.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AesCloudDataNet.Services
{
    public interface IUserService :
          IDalAbstractService<string, User>
    {

    }
 
    public class UserService : DalAbstractService<string, User>,
                        IUserService
    {
        //const bool USE_DB = false;
        //const bool USE_HTTP = false;

        // private readonly HttpClient Client;
        ILogger<UserService> Log;
    
        public UserService(ILogger<UserService> logger)
            : base(logger , 0) //USer stores Endless
        {
            Log = logger;
            //Client = new HttpClient();
        }
        #region Facade
   
        public override async Task<User> Get(string name, bool usePersist)
        {
            return await base.Get(name, usePersist);
        }
        public override async Task<List<User>> List(bool usePersist)
        {
            return await base.List(usePersist);
        }

        public override async Task<User> Insert(string name, User valueIn, bool usePersist)
        {
            return await base.Insert(name, valueIn,usePersist);
        }

    
        public override async Task<User> Update(string name, User valueIn, bool usePersist)
        {
            return await base.Update(name, valueIn, usePersist);
        }

        public override async Task Delete(string name, bool usePersist)
        {
            await base.Delete(name, usePersist);
        }

        #endregion

        #region Storage DB
        protected override async Task<User> RetrieveStorageItem(string name)
        {
            throw new NotImplementedException();
        }

        protected override async Task<Dictionary<string, User>> RetrieveStorageItemsList()
        {
            throw new NotImplementedException();
        }

        protected override async Task TryDeleteStorageItem(string name)
        {
            throw new NotImplementedException();
        }

        protected override async Task<User> TryInserStoragetItem(string name, User valueIn)
        {
            throw new NotImplementedException();
        }

        protected override async Task<User> TryUpdateStorageItem(string name, User valueIn)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
