using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserToken
    {
        [JsonProperty("user_login")]
        public string user_login { get; set; }
        [JsonProperty("expiry")]
        public DateTime expiry { get; set; }
        [JsonProperty("login_type")]
        public LoginType login_type { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("is_active")]
        public bool is_active { get; set; }
        [JsonProperty("nama")]
        public string nama { get; set; }
        [JsonProperty("type_user")]
        public TypeUser type_user { get; set; }
        [JsonProperty("id_user_datas")]
        public Guid id_user_datas { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("Expiry")]
        public DateTime Expiry { get; set; }
        [JsonProperty("LastLogin")]
        public DateTime LastLogin { get; set; }
        [JsonProperty("LoginType")]
        public LoginType LoginType { get; set; }
        [JsonProperty("id_subscriber")]
        public Guid id_subscriber { get; set; }
    }

    public class AuthTokenLogin
    {
        [JsonProperty("value_token")]
        public string value_token { get; set; }
    }

    public enum TypeUser
    {
        admin,
        client
    }
}
