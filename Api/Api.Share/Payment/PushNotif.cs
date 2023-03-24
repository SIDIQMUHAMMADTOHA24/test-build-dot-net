using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Payment
{
    public class PushNotif
    {
        [JsonProperty("transaction_time")]
        public string transaction_time { get; set; }
        [JsonProperty("transaction_status")]
        public string transaction_status { get; set; }
        [JsonProperty("transaction_id")]
        public string transaction_id { get; set; }
        [JsonProperty("status_message")]
        public string status_message { get; set; }
        [JsonProperty("status_code")]
        public string status_code { get; set; }
        [JsonProperty("signature_key")]
        public string signature_key { get; set; }
        [JsonProperty("payment_type")]
        public string payment_type { get; set; }
        [JsonProperty("order_id")]
        public string order_id { get; set; }
        [JsonProperty("merchant_id")]
        public string merchant_id { get; set; }
        [JsonProperty("gross_amount")]
        public string gross_amount { get; set; }
        [JsonProperty("fraud_status")]
        public string fraud_status { get; set; }
        [JsonProperty("currency")]
        public string currency { get; set; }

    }
}
