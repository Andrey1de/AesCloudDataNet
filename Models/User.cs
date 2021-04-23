using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudDataNet.Models
{
    public partial class User
    {
        //private string name;
        private Guid? guid;
        //private int _hashCode = 0;

        [Key]
       // [JsonIgnore]
      
        public string Name
        {
            get ;
            set ;
        }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public Guid? Guid { get => guid = guid ?? System.Guid.NewGuid(); set => guid = value; }
        public int Severity { get; set; }
        //public override int GetHashCode()
        //{
        //    return _hashCode;
        //}
    }
}
