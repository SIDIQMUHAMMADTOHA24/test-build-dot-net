using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.ESignApi
{
    public class UserPhrase
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("iduser")]
        public string iduser { get; set; }
        [JsonProperty("phrase")]
        public string phrase { get; set; }
        [JsonProperty("create_date")]
        public DateTime create_date { get; set; }
        [JsonProperty("update_date")]
        public DateTime update_date { get; set; }
        [JsonProperty("serial_number")]
        public string serial_number { get; set; }
    }
}
