using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserLoginDatas
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("user_name")]
        public string user_name { get; set; }
        [JsonProperty("token")]
        public string token { get; set; }
        [JsonProperty("expiry")]
        public DateTime expiry { get; set; }
        [JsonProperty("last_login")]
        public DateTime last_login { get; set; }
        [JsonProperty("user_name_hash")]
        public string user_name_hash { get; set; }
    }
}
