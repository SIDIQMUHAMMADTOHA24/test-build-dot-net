using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class UserAccess
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("id_user_package")]
        public Guid id_user_package { get; set; }
        [JsonProperty("sum_access")]
        public long sum_access { get; set; }
        [JsonProperty("date_update")]
        public DateTime date_update { get; set; }
        [JsonProperty("user_data_nama")]
        public string user_data_nama { get; set; }
        [JsonProperty("user_package")]
        public string user_package { get; set; }
        [JsonProperty("package")]
        public TypePackage package { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
    }
}
