using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudData
{
    public partial class User
    {
        [Key]
        public string Name { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? Guid { get; set; }
        public int Severity { get; set; }

       
    }
}
