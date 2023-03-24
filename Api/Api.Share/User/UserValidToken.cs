using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserValidToken
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("user_name")]
        public string user_name { get; set; }
        [JsonProperty("valid_token")]
        public string valid_token { get; set; }
        [JsonProperty("create_date")]
        public DateTime create_date { get; set; }
        [JsonProperty("active_date")]
        public DateTime active_date { get; set; }
        [JsonProperty("is_active")]
        public bool is_active { get; set; }
        [JsonProperty("type_validate")]
        public TypeValidate type_validate { get; set; }
    }

    public enum TypeValidate 
    { 
        client, 
        server
    }

}
