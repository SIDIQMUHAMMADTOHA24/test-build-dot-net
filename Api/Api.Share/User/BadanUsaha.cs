using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class BadanUsaha
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("nama")]
        public string nama { get; set; }
    }
}
