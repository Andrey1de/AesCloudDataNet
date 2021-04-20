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
    public class UsersController : ControllerBase
    {
        private readonly ClouddataContext _context;

        public UsersController(ClouddataContext context)
        {
            _context = context;
        }
       void  norm(User that)
        {
            that.UserId = that.Name.ToUpper().GetHashCode();
            that.Guid = that.Guid ?? System.Guid.NewGuid();
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{name}")]
        public async Task<ActionResult<User>> GetUser(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                        u => String.Compare(name, u.Name, true) == 0);

            if (user == null)
            {
                return NotFound("User:" + name);
            }

            return user;
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

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(name))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> NewUser(User user)
        {
            norm(user);
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Name))
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

        // DELETE: api/Users/5
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteUser(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                        u => String.Compare(name, u.Name, true) == 0);
            if (user == null)
            {
                return NotFound("User:" + name);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string name)
        {
            return _context.Users.Any(e => e.Name == name);
        }
    }
}
