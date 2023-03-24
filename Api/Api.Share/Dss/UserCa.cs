using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class UserCa : NameCa
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("password_ejbca")]
        public string password_ejbca { get; set; }
        [JsonProperty("subject_dn")]
        public string subject_dn { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("nama")]
        public string nama { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("telepon")]
        public string telepon { get; set; }
        [JsonProperty("kota")]
        public string kota { get; set; }
        [JsonProperty("provinsi")]
        public string provinsi { get; set; }
        [JsonProperty("status_user")]
        public string status_user { get; set; }
        [JsonProperty("serial_number")]
        public string serial_number { get; set; }
    }

    public class StatusUser
    {
        public const string ACTIVE = "ACTIVE";
        public const string NOT_ACTIVE = "NOT_ACTIVE";
    }
}
