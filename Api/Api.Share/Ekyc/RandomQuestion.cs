using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Ekyc
{
    public class RandomQuestion
    {
        [JsonProperty("id")]
        public Guid id { get; set; }
        [JsonProperty("question")]
        public string question { get; set; }
        [JsonProperty("answer")]
        public string answer { get; set; }
    }

    public class VideoAndQuestion : RandomQuestion
    {
        [JsonProperty("base64_video")]
        public string base64_video { get; set; }
        [JsonProperty("base64_image_face_target")]
        public string base64_image_face_target { get; set; }
        [JsonProperty("base64_image_id_card")]
        public string base64_image_id_card { get; set; }
        [JsonProperty("base64_akta_pendirian")]
        public string base64_akta_pendirian { get; set; }
        [JsonProperty("base64_surat_pengesahan")]
        public string base64_surat_pengesahan { get; set; }
        [JsonProperty("base64_surat_permohonan")]
        public string base64_surat_permohonan { get; set; }
        [JsonProperty("base64_surat_pelengkap")]
        public string base64_surat_pelengkap { get; set; }
        [JsonProperty("base64_verif_manual")]
        public string base64_verif_manual { get; set; }
        [JsonProperty("confidence")]
        public string confidence { get; set; }
    }
}
