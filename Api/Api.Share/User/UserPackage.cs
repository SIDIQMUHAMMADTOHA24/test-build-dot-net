using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserPackage
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("package")]
        public TypePackage package { get; set; }
        [JsonProperty("expired_date")]
        public DateTime expired_date { get; set; }
        [JsonProperty("package_string")]
        public string package_string { get; set; }
    }

    public enum TypePackage
    {
        enkripa_sign = 1, 
        enkripa_ekyc = 2,
        enkripa_ocr = 3
    }
}
