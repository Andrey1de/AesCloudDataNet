using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudDataNet.Models
{
    public partial class UserAction
    {
        private string user = "";
        private Guid guid = Guid.Empty;

        [JsonIgnore]
        [Key]
        public int UserActionId { get => guid.GetHashCode(); set { } }
        public  Guid Guid{ get => 
                guid = (guid != System.Guid.Empty) ? guid : System.Guid.NewGuid();
                 set => guid = value; }
        public string Type { get; set; }
        public DateTime? NextActionDate { get; set; }
        public int UserId { get => user.ToUpper().GetHashCode(); }
        public string User { get => user; set => user = value; }
        public int? PriodSec { get; set; }
        public byte[] Blob { get; set; }
        public string Json { get; set; }
    }
}
