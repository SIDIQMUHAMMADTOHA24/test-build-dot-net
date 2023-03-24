using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class PostRevoke
    {
        [JsonProperty("revocation_reason")]
        public string revocation_reason { get; set; }
        [JsonProperty("serial_number")]
        public string serial_number { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
    }

    public class RevocationReason
    {
        public const string UNSPECIFIED = "UNSPECIFIED";
        public const string CERTIFICATE_HOLD = "CERTIFICATE_HOLD";
        public const string KEY_COMPROMISE = "KEY_COMPROMISE";
        public const string AFFILITION_CHANGED = "AFFILITION_CHANGED";
    }
}
