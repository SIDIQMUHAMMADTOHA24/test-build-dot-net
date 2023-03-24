using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class VerifyFile
    {
        [JsonProperty("data_type")]
        public string data_type { get; set; } = "PDF";
        [JsonProperty("base64data")]
        public string base64data { get; set; }
    }

    public class VerifyData
    {
        [JsonProperty("summary")]
        public string summary { get; set; }
        [JsonProperty("notes")]
        public string notes { get; set; }
        [JsonProperty("details")]
        public List<VerifyDetail> details { get; set; }
    }

    public class VerifyDetail
    {
        [JsonProperty("signature_document")]
        public VerifySignatureDoc signature_document { get; set; }
        [JsonProperty("info_signer")]
        public VerifySignerInfo info_signer { get; set; }
        [JsonProperty("info_ca")]
        public List<VerifyCaInfo> info_ca { get; set; }
        [JsonProperty("info_tsa")]
        public VerifyTsaInfo info_tsa { get; set; }
    }

    public class VerifySignatureDoc
    {
        [JsonProperty("reason")]
        public string reason { get; set; }
        [JsonProperty("location")]
        public string location { get; set; }
        [JsonProperty("signed_in")]
        public string signed_in { get; set; }
    }
    public class VerifySignerInfo
    {
        [JsonProperty("signer_name")]
        public string signer_name { get; set; }
        [JsonProperty("signer_dn")]
        public string signer_dn { get; set; }
        [JsonProperty("issuer_dn")]
        public string issuer_dn { get; set; }
        [JsonProperty("serial")]
        public string serial { get; set; }
        [JsonProperty("signer_cert_validity")]
        public string signer_cert_validity { get; set; }
    }

    public class VerifyCaInfo
    {
        [JsonProperty("common_name")]
        public string common_name { get; set; }
        [JsonProperty("cert_validity")]
        public string cert_validity { get; set; }
        [JsonProperty("subject_dn")]
        public string subject_dn { get; set; }
        [JsonProperty("issuer_dn")]
        public string issuer_dn { get; set; }
        [JsonProperty("serial")]
        public string serial { get; set; }
    }

    public class VerifyTsaInfo
    {
        [JsonProperty("common_name")]
        public string common_name { get; set; }
        [JsonProperty("cert_validity")]
        public string cert_validity { get; set; }
        [JsonProperty("subject_dn")]
        public string subject_dn { get; set; }
        [JsonProperty("issuer_dn")]
        public string issuer_dn { get; set; }
        [JsonProperty("serial")]
        public string serial { get; set; }
    }
}
