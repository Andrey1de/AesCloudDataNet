using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AesCloudDataNet.DB;
using AesCloudDataNet.Models;
using Microsoft.Extensions.Logging;
using AesCloudDataNet.Services;
using System.Threading;

namespace AesCloudDataNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateToUsdController : ControllerBase
    {
        private readonly AesCloudDataContext Context;
        private readonly ILogger<RateToUsdController> Log;
        private readonly IRateToUsdService Svc;
        bool norm(ref string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length != 3)
                return false;

            name = name.Trim().ToUpper();
            return true;
        }
         public RateToUsdController(AesCloudDataContext context, 
            ILogger<RateToUsdController> logger,
            IRateToUsdService svc
            )
        {
            Context = context;
             Log = logger;
            Svc = svc;
            InitDb();
        }
        static int Begin = 1;

        void InitDb()
        {
            if(Interlocked.Exchange(ref Begin,0) != 0)
            {
                Dictionary<string, RateToUsd> dict =  Context.RateToUsd.ToDictionary(k => k.Code, p => p);
                Svc.AddRange(dict);
            }
        }
        // GET: api/RateToUsd
        [HttpGet]
        public async Task<ActionResult<List<RateToUsd>>> GetRateToUsds()
        {
             return Svc.List();
        }

        // GET: api/RateToUsd/5
        [HttpGet("{code}")]
        public async Task<ActionResult<RateToUsd>> GetRateToUsd(string code)
        {
            if (!norm(ref code))
            {
                return BadRequest($"Code {code} have been 3 letter length");
            }
            var rateToUsd = await Svc.Get(code, true);

            var rateToUsd1 = Context.RateToUsd.Find(code);

            if (rateToUsd == null )
            {
                if(rateToUsd1 != null)
                {
                    Context.RateToUsd.Remove(rateToUsd);
                    await Context.SaveChangesAsync();

                }
                return NotFound();
            }
            else if (rateToUsd1 == null)
            {
                Context.RateToUsd.Add(rateToUsd);
                await Context.SaveChangesAsync();

            }
            else if (rateToUsd.Rate != rateToUsd1.Rate
                || rateToUsd.Stored != rateToUsd1.Stored)
            {
                Context.Entry(rateToUsd).State = EntityState.Modified;
                await Context.SaveChangesAsync();
            }


            return rateToUsd;
        }

        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<FromTo>> GetRateToUsd(string from,string to)
        {
            if (!norm(ref from))
            {
                return BadRequest($"From {from}  have been 3 letter length");
            }
            if (!norm(ref to))
            {
                return BadRequest($"To {to} have been 3 letter length");
            }
            RateToUsd _from = await Svc.Get(from, true);
            RateToUsd _to = await Svc.Get(from, true);

            if (_from == null )
            {
                return NotFound($"Code {from} not found");
            }
            if ( _to == null)
            {
                return NotFound($"Code {to} not found");
            }
            if (_from == null || _to.Rate == 0)
            {
                return NotFound();
            }

            return new FromTo() { From = _from, To = _to };
        }

        // DELETE: api/RateToUsd/5
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteRateToUsd(string code)
        {
           
            RateToUsd rateToUsd = await Context.RateToUsd.FindAsync(code);
            if (rateToUsd!=null)
            {
                Context.RateToUsd.Remove(rateToUsd);
                await Context.SaveChangesAsync();
            }

            if (!Svc.Delete(code))
            {
                return NotFound();
            }


            return Ok();
        }

        private bool RateToUsdExists(string code)
        {
            return Context.RateToUsd.Any(e => e.Code == code);
        }
    }
}
