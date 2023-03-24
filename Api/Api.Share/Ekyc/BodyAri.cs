using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class BodyARI
    {
        [JsonProperty("trx_id")]
        public string trx_id { get; set; }
        [JsonProperty("nik")]
        public string nik { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("birthdate")]
        public string birthdate { get; set; }
        [JsonProperty("birthplace")]
        public string birthplace { get; set; }
        [JsonProperty("identity_photo")]
        public string identity_photo { get; set; }
    }
}
