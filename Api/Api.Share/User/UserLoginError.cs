using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserLoginError
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("user_name")]
        public string user_name { get; set; }
        [JsonProperty("sum")]
        public int sum { get; set; }
        [JsonProperty("update_date")]
        public DateTime update_date { get; set; }
    }
}
