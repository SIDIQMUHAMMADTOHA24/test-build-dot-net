using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class ResponseARI
    {
        [JsonProperty("timestamp")]
        public long timestamp { get; set; }
        [JsonProperty("status")]
        public int status { get; set; }
        [JsonProperty("data")]
        public ResponseData data { get; set; }
        [JsonProperty("trx_id")]
        public string trx_id { get; set; }
        [JsonProperty("ref_id")]
        public string ref_id { get; set; }
    }

    public class ResponseData
    {
        [JsonProperty("name")]
        public bool name { get; set; }
        [JsonProperty("birthdate")]
        public bool birthdate { get; set; }
        [JsonProperty("birthplace")]
        public bool birthplace { get; set; }
        [JsonProperty("address")]
        public string address { get; set; }
    }
}
