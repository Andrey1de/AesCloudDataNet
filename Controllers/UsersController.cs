using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AesCloudDataNet.Models;
using AesCloudDataNet.Services;
using Microsoft.Extensions.Logging;
using AesCloudDataNet.Exceptions;


namespace AesCloudDataNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        const bool USE_DB = false;
        //const bool USE_HTTP = false;
        readonly private ILogger<UsersController> Log;

        //      private readonly ClouddataContext _context;
        private readonly IUserService Srv;

        public UsersController(ILogger<UsersController> logger,
            IUserService service)//ClouddataContext context)
        {
            Log = logger;
            Srv = service;
           // _context = context;
        }
        void  norm(User that)
        {

             that.Guid = that.Guid ?? System.Guid.NewGuid();
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await Srv.List(USE_DB);
        }

        // GET: api/Users/5
        [HttpGet("{name}")]
        public async Task<ActionResult<User>> GetUser(string name)
        {
            var user = await Srv.Get(name, USE_DB);

            if (user == null)
            {
                return NotFound("User:" + name);
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> NewUser(User user)
        {
            norm(user);
            if (Srv.HasItem(user.Name))
            {
                return Conflict($"INSERT: User {user.Name} just exists");
            }
            try
            {
                await Srv.Insert(user.Name, user, USE_DB);
               
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
                if (Srv.HasItem(user.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Name }, user);
        }
        // PUT: api/Users/john@doe
        [HttpPut("{name}")]
        public async Task<ActionResult<User>> UpdateUser(string name, User user)
        {
            norm(user);
            if (String.Compare(name, user.Name, true) != 0)
            {
                return BadRequest();
            }

            if (!Srv.HasItem(user.Name))
            {
                return NotFound($"UPDATE User:{name} ");
            }

            try
            {
                await Srv.Update(name, user, USE_DB);
            }
            catch (StoreException ex)
            {
                Log.LogError(ex.Message, ex);
                return Conflict();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);

                if (!Srv.HasItem(name))
                {
                    return NotFound();
                }
                else
                {
                    return UnprocessableEntity();
                }
            }

            return Ok(user);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
 

        // DELETE: api/Users/5
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteUser(string name)
        {
          
            if (!Srv.HasItem(name))
            {
                return NotFound($"DELETE User:{name} ");
            }

            try
            {
                await Srv.Delete(name,  USE_DB);
            }
            catch (StoreException ex)
            {
                Log.LogError(ex.Message, ex);
                return Conflict();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);

                if (!Srv.HasItem(name))
                {
                    return NotFound();
                }
                else
                {
                    return UnprocessableEntity();
                }
            }


            return Ok();
        }

       
    }
}
