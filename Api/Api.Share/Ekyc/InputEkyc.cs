using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class InputEkyc
    {
        [JsonProperty("name_image_id_card")]
        public string name_image_id_card { get; set; }
        [JsonProperty("name_image_face_target")]
        public string name_image_face_target { get; set; }
        [JsonProperty("name_video")]
        public string name_video { get; set; }
        [JsonProperty("base64_image_id_card")]
        public string base64_image_id_card { get; set; }
        [JsonProperty("base64_image_face_target")]
        public string base64_image_face_target { get; set; }
        [JsonProperty("base64_video")]
        public string base64_video { get; set; }
    }

    public class InputEkycPR : InputEkyc
    {
        [JsonProperty("name_akta_pendirian")]
        public string name_akta_pendirian { get; set; }
        [JsonProperty("name_surat_pengesahan")]
        public string name_surat_pengesahan { get; set; }
        [JsonProperty("name_surat_permohonan")]
        public string name_surat_permohonan { get; set; }
        [JsonProperty("base64_akta_pendirian")]
        public string base64_akta_pendirian { get; set; }
        [JsonProperty("base64_surat_pengesahan")]
        public string base64_surat_pengesahan { get; set; }
        [JsonProperty("base64_surat_permohonan")]
        public string base64_surat_permohonan { get; set; }
    }

    public class InputVerifManual
    {
        [JsonProperty("name_file_manual")]
        public string name_file_manual { get; set; }
        [JsonProperty("base64_file_manual")]
        public string base64_file_manual { get; set; }
        [JsonProperty("id_ekyc")]
        public Guid id_ekyc { get; set; }
    }
}
