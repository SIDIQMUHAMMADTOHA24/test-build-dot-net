using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.User
{
    public class SumDash
    {
        [JsonProperty("sum_status")]
        public List<long> sum_status { get; set; }
        [JsonProperty("list_sum_per_weeks")]
        public List<SumPerWeeks> list_sum_per_weeks { get; set; }
    }

    public class SumPerWeeks
    {
        [JsonProperty("count")]
        public long count { get; set; }
        [JsonProperty("string")]
        public string day { get; set; }
    }

}
