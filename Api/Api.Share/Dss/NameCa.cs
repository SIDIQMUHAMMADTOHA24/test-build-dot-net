using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class NameCa
    {
        [JsonProperty("ca_name")]
        public string ca_name { get; set; }
        [JsonProperty("cp_name")]
        public string cp_name { get; set; }
        [JsonProperty("ee_name")]
        public string ee_name { get; set; }
    }
}
