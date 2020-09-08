using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class IdentifyPersonRespone
    {
        [JsonPropertyName("faceId")]
        public string FaceId { get; set; }
        [JsonPropertyName("candidates")]
        public List<IdentifyCandidate> Candidates { get; set; }
    }
}
