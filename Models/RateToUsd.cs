using System;
using System.Collections.Generic;

#nullable disable

namespace AesCloudData
{
    public partial class RateToUsd
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public DateTime Stored { get; set; }
        public DateTime LastRefreshed { get; set; }
    }
}
