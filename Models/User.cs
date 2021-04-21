using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace AesCloudDataNet.Models
{
    public partial class User
    {
        private string name;
        private Guid? guid;
        private int _hashCode = 0;

        [JsonIgnore]
        public int Id { get => _hashCode; set { } }
        public string Name
        {
            get => name;
            set { 
                name = value.ToUpper();
                _hashCode = name.GetHashCode();
              } 
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? Guid { get => guid = guid ?? System.Guid.NewGuid(); set => guid = value; }
        public int Severity { get; set; }
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
