using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class UserEkyc
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("full_name")]
        public string full_name { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("phone")]
        public string phone { get; set; }
        [JsonProperty("time_input")]
        public DateTime time_input { get; set; }
        [JsonProperty("time_edit")]
        public DateTime time_edit { get; set; }
        [JsonProperty("status")]
        public StatusUserEkyc status { get; set; }
        [JsonProperty("status_string")]
        public string status_string { get; set; }
        [JsonProperty("confidence")]
        public string confidence { get; set; }
        [JsonProperty("id_random_question")]
        public Guid id_random_question { get; set; }
        [JsonProperty("id_user_datas")]
        public Guid id_user_datas { get; set; }
        [JsonProperty("user_datas_nama")]
        public string user_datas_nama { get; set; }
    }

    public enum StatusUserEkyc
    {
        not_active,
        active,
        pending
    }
}
