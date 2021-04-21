using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AesCloudDataNet.Services;
using AesCloudDataNet.Models;

namespace AesCloudDataNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        const bool TO_USE_DB = false;
 //       const bool TO_USE_HTTP = true;

        private readonly IExchangeRateService Dal;
        ILogger<ExchangeRateController> Log;
        //readonly bool 
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
        public async Task<ActionResult<List<RateToUsd>>> Get()
        {
            return await Dal.List(TO_USE_DB);
        }

        // GET: api/ExchangeRate/ILS
        [HttpGet("{code}")]
        public async Task<ActionResult<RateToUsd>> Get(string code)
        {
            if (!norm(ref code))
            {
                return BadRequest("code");
            }
            var rateToUsd = await Dal.Get(code, TO_USE_DB);

            if (rateToUsd == null)
            {
                return NotFound();
            }

            return rateToUsd;
        }


        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<FromTo>> GetPair(string from , string to)
        {
            if (!norm(ref from))
            {
                return BadRequest("bad parameter:from");
            }
            if (!norm(ref to))
            {
                return BadRequest("bad parameter:to");
            }

            RateToUsd _from = await Dal.Get(from, TO_USE_DB);
            RateToUsd _to = await Dal.Get(to, TO_USE_DB);
            if (_from == null)
            {
                return Problem($"Exchange rate for {from} not exists");
            }
            if (_to == null )
            {
                return Problem($"Exchange rate for {to} not exists");
            }
            if (_to.Rate == 0.0)
            {
                return Problem($"Exchange rate for {to} has 0 rate division is impossible");
            }


            FromTo ret = new FromTo() { From = _from, To = _to};

            return ret;
        }

        // DELETE: api/ExchangeRate/5
        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            if (!norm(ref code))
            {
                return BadRequest();
            }

            await Dal.Delete(code, TO_USE_DB);

       
            return Ok();
        }

    
    }
}
