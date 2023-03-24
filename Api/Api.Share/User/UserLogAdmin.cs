using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserLogAdmin
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("user_name")]
        public string user_name { get; set; }
        [JsonProperty("activity")]
        public string activity { get; set; }
        [JsonProperty("time_input")]
        public DateTime time_input { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
    }
}
