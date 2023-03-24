using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserFromEkyc
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("id_ekyc")]
        public Guid id_ekyc { get; set; }
    }
}
