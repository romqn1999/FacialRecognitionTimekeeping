using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class TimekeepingRecord
    {
        [Required]
        public string AliasId { get; set; }
        [JsonIgnore]
        public TimekeepingPerson TimekeepingPerson { get; set; }
        [Required]
        public long TimekeepingRecordUnixTimestampSeconds { get; set; }
    }
}
