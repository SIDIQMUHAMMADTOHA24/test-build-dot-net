using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class MsgResponse<T>
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("data")]
        public T data { get; set; }
    }

    public class MsgResponseData<T>
    {
        //[JsonProperty("filters")]
        //public string filters { get; set; }
        [JsonProperty("iTotalRecords")]
        public long iTotalRecords { get; set; }
        [JsonProperty("iTotalDisplayRecords")]
        public long iTotalDisplayRecords { get; set; }
        [JsonProperty("aaData")]
        public T aaData { get; set; }
    }

    public class GetList
    {
        //[JsonProperty("filters")]
        //public string filters { get; set; }
        [JsonProperty("iDisplayLength")]
        public int iDisplayLength { get; set; }
        [JsonProperty("iDisplayStart")]
        public int iDisplayStart { get; set; }
        [JsonProperty("search")]
        public string search { get; set; }
    }
}
