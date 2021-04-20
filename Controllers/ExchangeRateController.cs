using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AesCloudData;
using Microsoft.Extensions.Logging;
using AesCloudDataNet.Services;

namespace AesCloudData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService Dal;
        ILogger<ExchangeRateController> Log;
        readonly bool 
        bool norm(ref string code)
        {
            code = (code ?? "").ToUpper();
            return code.Length >= 3;
        }
        bool norm(RateToUsd that)
        {
            that.Code = (that.Code ?? "").ToUpper();
            return that.Code.Length >= 3;
        }
        public ExchangeRateController(IExchangeRateService dal, ILogger<ExchangeRateController> logger)
        {
            Log = logger;
            Dal = dal;
        }

        // GET: api/ExchangeRate
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RateToUsd>>> GetRateToUsds()
        {
            return await Dal.List();
        }

        // GET: api/ExchangeRate/ILS
        [HttpGet("{code}")]
        public async Task<ActionResult<RateToUsd>> GetRateToUsd(string code)
        {
            if(!norm(ref code))
            {
                return BadRequest();
            }
            var rateToUsd = await Dal.RateToUsds.FindAsync(code);

            if (rateToUsd == null)
            {
                return NotFound();
            }

            return rateToUsd;
        }

        // PUT: api/ExchangeRate/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkcode=2123754
        [HttpPut("{code}")]
        public async Task<ActionResult<RateToUsd>> UpdateRateToUsd(string code, RateToUsd rateToUsd)
        {

            if (!norm(ref code) || !norm(rateToUsd) || code != rateToUsd.Code)
            {
                return BadRequest();
            }

            Dal.Entry(rateToUsd).State = EntityState.Modified;

            try
            {
                await Dal.SaveChangesAsync();
                return Ok(rateToUsd);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RateToUsdExists(code))
                {
                    return NotFound();
                }
                else
                {
                    Log.LogError(ex.StackTrace);
                   // throw ex;
                }
            }
            catch(Exception ex)
            {
                Log.LogError(ex.StackTrace);

            }

            return Conflict();
        }

        // POST: api/ExchangeRate
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkcode=2123754
        [HttpPost]
        public async Task<ActionResult<RateToUsd>>NewRateToUsd(RateToUsd rateToUsd)
        {
            if ( !norm(rateToUsd) )
            {
                return BadRequest();
            }

            Dal.RateToUsds.Add(rateToUsd);
            try
            {
                await Dal.SaveChangesAsync();
                return CreatedAtAction("GetRateToUsd", new { code = rateToUsd.Code }, rateToUsd);
            }
            catch (DbUpdateException ex)
            {
                if (RateToUsdExists(rateToUsd.Code))
                {
                    return Conflict();
                }
                else
                {
                    Log.LogError(ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.StackTrace);
            }

            return Conflict();
        }

        // DELETE: api/ExchangeRate/5
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteRateToUsd(string code)
        {
            if (!norm(ref code))
            {
                return BadRequest();
            }

            var rateToUsd = await Dal.RateToUsds.FindAsync(code);
            if (rateToUsd == null)
            {
                return NotFound();
            }

            Dal.RateToUsds.Remove(rateToUsd);
            await Dal.SaveChangesAsync();

            return NoContent();
        }

        private bool RateToUsdExists(string code)
        {
            return Dal.RateToUsds.Any(e => e.Code == code);
        }
    }
}
