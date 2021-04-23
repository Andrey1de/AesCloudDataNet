using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudDataNet.Models
{
    public class RateToUsd
    {
     
        //[JsonIgnore]
        [Key]
        public string Code { get ; set ; }
        public string Name { get; set; }
        public double Rate { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public DateTime Stored { get; set; }
        public DateTime LastRefreshed { get; set; }

        //void normCode(string codeIn)  {
        //    _code = (codeIn ?? "").ToUpper().Substring(0, 3);
        //}

        //public override int GetHashCode()
        //{
        //    if(_hash == 0)
        //    {
        //        Array.ForEach(_code.ToCharArray(), p => { _hash = (_hash << 8) + (byte)p; });
        //    }


        //    return _hash;
        //}
        public static readonly RateToUsd RateUsdToUsd = new RateToUsd()
        {
            Code = "USD",
            Name = "USD Dollar",
            Ask = 1.0,
            Bid = 1.0,
            Rate = 1.0,
            LastRefreshed = DateTime.Now.AddDays(-5),
            Stored = DateTime.Now.AddDays(-3)


        };

    }
}

