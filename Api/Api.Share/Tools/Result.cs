using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Tools
{
    public class Result
    {
        public class GetResult<T>
        {
            [JsonProperty("draw")]
            public long draw { get; set; }
            [JsonProperty("recordsTotal")]
            public long recordsTotal { get; set; }
            [JsonProperty("recordsFiltered")]
            public long recordsFiltered { get; set; }
            [JsonProperty("data")]
            public List<T> data { get; set; }
        }

        public class PostResult
        {
            [JsonProperty("result")]
            public bool result { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
        }

        public class PostResultEkyc
        {
            [JsonProperty("result")]
            public bool result { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
            [JsonProperty("face_compare")]
            public string face_compare { get; set; }
        }

        public class PostResultObject<T>
        {
            [JsonProperty("result")]
            public bool result { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
            [JsonProperty("objek")]
            public T objek { get; set; }
        }

        public class GetResultObject<T>
        {
            [JsonProperty("result")]
            public bool result { get; set; }
            [JsonProperty("objek")]
            public T objek { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
        }

        public class GetResultData<T> : GetResultObject<T>
        {
            [JsonProperty("total")]
            public long total { get; set; }
        }

        public class PostResultValid
        {
            [JsonProperty("valid")]
            public bool valid { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
        }
    }
}
