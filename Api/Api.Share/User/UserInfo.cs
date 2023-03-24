using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserInfo
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("email_address")]
        public string email_address { get; set; }
        [JsonProperty("phone_number")]
        public string phone_number { get; set; }
        [JsonProperty("payment")]
        public TypePayment payment { get; set; }
        [JsonProperty("license")]
        public string license { get; set; }
        [JsonProperty("input_date")]
        public DateTime input_date { get; set; }
        [JsonProperty("update_date")]
        public DateTime update_date { get; set; }
        [JsonProperty("status")]
        public StatusUserInfo status { get; set; }
        [JsonProperty("status_string")]
        public string status_string { get; set; }
        [JsonProperty("id_ekyc")]
        public Guid id_ekyc { get; set; }
        [JsonProperty("id_subscriber")]
        public Guid id_subscriber { get; set; }
        [JsonProperty("nik")]
        public string nik { get; set; }
    }

    public enum StatusUserInfo
    {
        non_active,
        active,
        pending
    }

    public enum TypePayment
    {
        transfer,
        credit_card,
    }
}
