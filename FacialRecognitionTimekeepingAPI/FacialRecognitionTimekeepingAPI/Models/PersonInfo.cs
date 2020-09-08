using System.Text.Json.Serialization;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class PersonInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("personId")]
        public string PersonId { get; set; }
        [JsonPropertyName("userData")]
        public string UserData { get; set; }
    }
}