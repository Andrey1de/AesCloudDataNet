using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudData
{
    public partial class UserAction
    {
        
        [JsonIgnore]
        public int Id { get; set; }
        public Guid? Guid { get; set; }
        public string Type { get; set; }
        public DateTime? NextActionDate { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int? PriodSec { get; set; }
        public byte[] Blob { get; set; }
        public string Json { get; set; }
        public override int GetHashCode()
        {
            return (Guid?.ToString() ?? "").GetHashCode();
        }
    }
}
