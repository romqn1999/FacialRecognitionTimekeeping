using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class IdentifyCandidate
    {
        [JsonPropertyName("personId")]
        public string PersonId { get; set; }
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
