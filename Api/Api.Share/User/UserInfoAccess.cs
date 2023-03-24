using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserInfoAccess
    {
        [JsonProperty("user_info")]
        public UserInfo user_info { get; set; }
        [JsonProperty("user_access")]
        public UserAccess user_access { get; set; }
    }
}
