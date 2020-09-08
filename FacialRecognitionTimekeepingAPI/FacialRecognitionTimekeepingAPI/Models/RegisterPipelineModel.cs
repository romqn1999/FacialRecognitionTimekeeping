using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class RegisterPipelineModel
    {
        public PersonInfo PersonInfo { get; set; }
        public byte[] FaceData { get; set; }
        public IFormFile FormFile { get; set; }
        public string AliasId { get; set; }
        public Services.TimekeepingContext TimekeepingContext { get; set; }
        public string Message { get; set; }
        public Face BiggestFace { get; set; }
        public bool HasError { get; set; } = false;
        public TimekeepingPerson TimekeepingPerson { get; set; }

        public void SetError(string message)
        {
            HasError = true;
            Message = message;
        }
    }
}
