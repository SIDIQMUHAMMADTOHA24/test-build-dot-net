using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class TokenAccess
    {
        [JsonProperty("token")]
        public string token { get; set; }
    }
}
