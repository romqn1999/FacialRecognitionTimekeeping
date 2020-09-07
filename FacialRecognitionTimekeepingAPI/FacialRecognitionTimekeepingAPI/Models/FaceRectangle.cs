using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class FaceRectangle
    {
        [JsonPropertyName("top")]
        public double Top { get; set; }
        [JsonPropertyName("left")]
        public double Left { get; set; }
        [JsonPropertyName("width")]
        public double Width { get; set; }
        [JsonPropertyName("height")]
        public double Height { get; set; }
    }
}
