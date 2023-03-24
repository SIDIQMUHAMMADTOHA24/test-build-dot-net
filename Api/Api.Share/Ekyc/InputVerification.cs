using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class InputVerification
    {
        [JsonProperty("input_ekyc")]
        public InputEkyc input_ekyc { get; set; }
        [JsonProperty("user_ekyc")]
        public UserEkyc user_ekyc { get; set; }
        [JsonProperty("type_ekyc")]
        public TypeEkyc type_ekyc { get; set; }
        [JsonProperty("body_ra")]
        public BodyARI body_ra { get; set; }
    }

    public class InputVerificationPR
    {
        [JsonProperty("input_ekyc")]
        public InputEkycPR input_ekyc { get; set; }
        [JsonProperty("user_ekyc")]
        public UserEkyc user_ekyc { get; set; }
        [JsonProperty("type_ekyc")]
        public TypeEkyc type_ekyc { get; set; }
        [JsonProperty("body_ra")]
        public BodyARI body_ra { get; set; }
    }

    public enum TypeEkyc
    {
        no_video,
        video
    }
}
