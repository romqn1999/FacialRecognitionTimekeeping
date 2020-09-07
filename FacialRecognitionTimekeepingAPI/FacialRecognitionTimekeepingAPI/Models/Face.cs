using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class Face
    {
        [JsonPropertyName("faceId")]
        public string FaceId { get; set; }
        [JsonPropertyName("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
    }
}
