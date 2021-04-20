using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AesCloudData;

namespace AesCloudData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActionsController : ControllerBase
    {
        private readonly ClouddataContext _context;

        public UserActionsController(ClouddataContext context)
        {
            _context = context;
        }

        bool norm(UserAction that)
        {
            that.Guid = that.Guid ?? System.Guid.NewGuid();
            that.Id = that.Guid.ToString().GetHashCode();
            return true;
        }

            // GET: api/UserActions
         [HttpGet]
        public async Task<ActionResult<List<UserAction>>> GetUserActions()
        {
            return await _context.UserActions.ToListAsync();
        }
        //TBD new gets

        // GET: api/UserActions/{839BEC17-8B22-4761-811F-0FC6CD1EF419}
        [HttpGet("guid/{guid}")]
        public async Task<ActionResult<UserAction>> GetUserAction(Guid guid)
        {
            var id = guid.ToString().GetHashCode();
            var userAction = await _context.UserActions.FindAsync(id);

            if (userAction == null)
            {
                return NotFound();
            }

            return userAction;
        }

        // PUT: api/UserActions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<UserAction>> UpdateUserAction(int id, UserAction userAction)
        {
            try
            {
                norm(userAction);
                _context.Entry(userAction).State = EntityState.Modified;
                 await _context.SaveChangesAsync();
                return Ok(userAction);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UserActionExists(id))
                {
                    return NotFound(ex.Message);
                }
                else
                {
                    throw ;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        // POST: api/UserActions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAction>> NewUserAction(UserAction userAction)
        {
            try
            {

                norm(userAction);
               _context.UserActions.Add(userAction);
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (UserActionExists(userAction.Id))
                {
                    return Conflict(ex);
                }
                else
                {
                    throw ;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return CreatedAtAction("GetUserAction", new { id = userAction.Id }, userAction);
        }

        // DELETE: api/UserActions/5
        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteUserAction(Guid guid)
        {
            try
            {
                var id = guid.ToString().GetHashCode();
                var userAction = await _context.UserActions.FindAsync(id);
                if (userAction == null)
                {
                    return NotFound();
                }

                _context.UserActions.Remove(userAction);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                return Conflict(ex.Message);
            }
            catch(Exception)
            { 
                 throw;
            }
            return Ok();
        }

        private bool UserActionExists(int id)
        {
            return _context.UserActions.Any(e => e.Id == id);
        }
    }
}
