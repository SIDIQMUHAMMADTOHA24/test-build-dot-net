using Api.Share.Ekyc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class ForRegUser
    {
        [JsonProperty("input_verification")]
        public InputVerification input_verification { get; set; }
        [JsonProperty("user")]
        public User user { get; set; }
        [JsonProperty("token_data")]
        public string token_data { get; set; }
        [JsonProperty("token_otp")]
        public string token_otp { get; set; }
    }

    public class ForRegUserPR
    {
        [JsonProperty("input_verification")]
        public InputVerificationPR input_verification { get; set; }
        [JsonProperty("user")]
        public UserPR user { get; set; }
        [JsonProperty("token_data")]
        public string token_data { get; set; }
        [JsonProperty("token_otp")]
        public string token_otp { get; set; }
    }
}
