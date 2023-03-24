using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserToLogin
    {
        [JsonProperty("user_login")]
        public string user_login { get; set; }
        [JsonProperty("pass_login")]
        public string pass_login { get; set; }
    }

    public class UserToChangePassword : UserToLogin
    {
        [JsonProperty("new_pass_login")]
        public string new_pass_login { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
    }
}
