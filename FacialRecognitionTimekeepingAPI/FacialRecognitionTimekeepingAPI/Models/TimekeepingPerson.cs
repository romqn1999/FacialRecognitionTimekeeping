using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class TimekeepingPerson
    {
        [Key]
        [Required]
        public string AliasId { get; set; }
        [Required]
        public string CognitivePersonId { get; set; }
        [JsonIgnore]
        public List<TimekeepingRecord> TimekeepingRecords { get; set; }
    }
}
