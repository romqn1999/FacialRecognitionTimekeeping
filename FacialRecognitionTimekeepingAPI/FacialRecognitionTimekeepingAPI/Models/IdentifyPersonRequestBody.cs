using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class IdentifyPersonRequestBody
    {
        [JsonPropertyName("personGroupId")]
        public string PersonGroupId { get; set; }
        //[JsonPropertyName("largePersonGroupId")]
        //public string LargePersonGroupId { get; set; }
        [JsonPropertyName("faceIds")]
        public List<string> FaceIds { get; set; }
        //[JsonPropertyName("maxNumOfCandidatesReturned")]
        //public double MaxNumOfCandidatesReturned { get; set; }
        //[JsonPropertyName("confidenceThreshold")]
        //public double ConfidenceThreshold { get; set; }
    }
}
