using Api.Share.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Payment
{
    public class BankTrans
    {

        [JsonProperty("payment_type")]
        public string payment_type { get; set; }
        [JsonProperty("transaction_details")]
        public TransactionDetail transaction_details { get; set; }
        [JsonProperty("bank_transfer")]
        public BankTransfer bank_transfer { get; set; }
        [JsonProperty("echannel")]
        public Echannel echannel { get; set; }
    }

    public class TransactionDetail 
    {

        [JsonProperty("order_id")]
        public string order_id { get; set; }
        [JsonProperty("gross_amount")]
        public long gross_amount { get; set; }
        [JsonProperty("id_user_info")]
        public Guid id_user_info { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("input_date")]
        public DateTime input_date { get; set; }
        [JsonProperty("update_date")]
        public DateTime update_date { get; set; }
    }

    public class BankTransfer
    {

        [JsonProperty("bank")]
        public string bank { get; set; }
    }

    public class Echannel
    {

        [JsonProperty("bill_info1")]
        public string bill_info1 { get; set; } = Const.bill_info1;
        [JsonProperty("bill_info2")]
        public string bill_info2 { get; set; } = Const.bill_info2;
    }
}
