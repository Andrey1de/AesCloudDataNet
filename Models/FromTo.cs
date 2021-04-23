using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AesCloudDataNet.Models
{
    /// <summary>
    /// class for operate with From->To based operations 
    /// </summary>
    public class FromTo
    {
       // [Key]
        public string Pair => From.Code + "-" + To.Code;
        public double ExchangeRate => (To.Rate == 0) ? 0 : (From.Rate / To.Rate);

        public DateTime Updated => (From.Stored < To.Stored) ? From.Stored : To.Stored;
        public RateToUsd From { get; set; }
        public RateToUsd To { get; set; }



    }
}
