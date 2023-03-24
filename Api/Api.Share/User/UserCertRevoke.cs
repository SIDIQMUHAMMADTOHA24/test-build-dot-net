using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserCertRevoke
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("reason")]
        public string reason { get; set; }
        [JsonProperty("date_input")]
        public DateTime date_input { get; set; }
        [JsonProperty("date_update")]
        public DateTime date_update { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("id_ekyc")]
        public Guid id_ekyc { get; set; }
        [JsonProperty("user_info")]
        public UserInfo user_info { get; set; }
        [JsonProperty("status")]
        public StatusUserCertRevoke status { get; set; }
        [JsonProperty("status_value")]
        public string status_value { get; set; }
    }

    public enum StatusUserCertRevoke
    { 
        req_revoke, 
        revoke
    }

}
