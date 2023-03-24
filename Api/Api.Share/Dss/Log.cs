using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class Log
    {
        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("action")]
        public string action { get; set; }
    }
}
