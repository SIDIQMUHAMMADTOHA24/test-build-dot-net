using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class User
    {
        [JsonProperty("user_login_info")]
        public UserLoginInfo user_login_info { get; set; }
        [JsonProperty("user_info")]
        public UserInfo user_info { get; set; }
        [JsonProperty("list_user_package")]
        public List<UserPackage> list_user_package { get; set; }
        [JsonProperty("user_from_ekyc")]
        public UserFromEkyc user_from_ekyc { get; set; }
    }

    public class UserPR
    {
        [JsonProperty("user_login_info")]
        public UserLoginInfo user_login_info { get; set; }
        [JsonProperty("user_info")]
        public UserInfo user_info { get; set; }
        [JsonProperty("list_user_package")]
        public List<UserPackage> list_user_package { get; set; }
        [JsonProperty("user_from_ekyc")]
        public UserFromEkyc user_from_ekyc { get; set; }
        [JsonProperty("badan_usaha")]
        public BadanUsaha badan_usaha { get; set; }
    }
}
