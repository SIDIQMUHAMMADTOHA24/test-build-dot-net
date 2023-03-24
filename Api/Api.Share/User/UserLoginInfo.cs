using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserLoginInfo
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("user_name")]
        public string user_name { get; set; }
        [JsonProperty("password_login")]
        public string password_login { get; set; }
        [JsonProperty("is_active")]
        public bool is_active { get; set; }
        [JsonProperty("login_type")]
        public LoginType login_type { get; set; }
        [JsonProperty("input_date")]
        public DateTime input_date { get; set; }
        [JsonProperty("update_date")]
        public DateTime update_date { get; set; }
        [JsonProperty("active_date")]
        public DateTime active_date { get; set; }
        [JsonProperty("change_password_date")]
        public DateTime change_password_date { get; set; }
        [JsonProperty("id_subscriber")]
        public Guid id_subscriber { get; set; }
    }

    public enum LoginType
    {
        admin,
        superadmin,
        client = 100,
        statis = 99,
        User = 200,
        Service = 300,
        Public = 700,
        Other = 900
    }
}
