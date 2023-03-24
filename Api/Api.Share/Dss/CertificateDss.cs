using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Dss
{
    public class CertificateDss : NameCa
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("validity_start")]
        public DateTime validity_start { get; set; }
        [JsonProperty("validity_end")]
        public DateTime validity_end { get; set; }
        [JsonProperty("enroll_date")]
        public DateTime enroll_date { get; set; }
        [JsonProperty("revoke_date")]
        public DateTime? revoke_date { get; set; }
        [JsonProperty("renew_date")]
        public DateTime? renew_date { get; set; }
        [JsonProperty("serial_number")]
        public string serial_number { get; set; }
        [JsonProperty("subject_dn")]
        public string subject_dn { get; set; }
        [JsonProperty("issuer_dn")]
        public string issuer_dn { get; set; }
        [JsonProperty("message_enrollment")]
        public string message_enrollment { get; set; }
        [JsonProperty("csr")]
        public string csr { get; set; }
        [JsonProperty("sertifikat")]
        public string sertifikat { get; set; }
        [JsonProperty("request_id")]
        public string request_id { get; set; }
        [JsonProperty("passphrase")]
        public string passphrase { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
    }

    public class CertificateDssGet
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("validity_start")]
        public DateTime validity_start { get; set; }
        [JsonProperty("validity_end")]
        public DateTime validity_end { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("serial_number")]
        public string serial_number { get; set; }
    }

    public class CertificateDssLevel
    {
        public const string NOT_CERTIFIED = "NOT_CERTIFIED";
        public const string FORM_FILLING = "FORM_FILLING";
        public const string FORM_FILLING_AND_ANNOTATIONS = "FORM_FILLING_AND_ANNOTATIONS";
        public const string NO_CHANGES_ALLOWED = "NO_CHANGES_ALLOWED";
    }

    public class CertificateType
    {
        public const string DIGITAL_SIGNATURE = "DIGITAL_SIGNATURE";
        public const string ESEAL = "ESEAL";
    }
}
