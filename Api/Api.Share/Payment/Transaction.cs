using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Payment
{
    public class Transaction
    {
        [JsonProperty("transaction_details")]
        public TransactionDetail transaction_details { get; set; }
        [JsonProperty("customer_details")]
        public CustomerDetail customer_details { get; set; }
    }

    public class CustomerDetail
    {
        [JsonProperty("first_name")]
        public string first_name { get; set; }
        [JsonProperty("last_name")]
        public string last_name { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("phone")]
        public string phone { get; set; }
    }
}
