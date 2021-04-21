using System;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudDataNet.Models
{
    public class RateToUsd
    {
        private string code;
        [JsonIgnore]
        public int Id { get => code.GetHashCode(); set { } }
        public string Code { get => code; set => code = normCode(value); }
        public string Name { get; set; }
        public double Rate { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public DateTime Stored { get; set; }
        public DateTime LastRefreshed { get; set; }

        string normCode(string code) => (code ?? "").ToUpper().Substring(0, 3);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
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

