using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class RecognizeTimekeepingPipelineModels
    {
        public IFormFile FormFile { get; set; }
        public Services.TimekeepingContext TimekeepingContext { get; set; }
        public bool HasError { get; set; } = false;
        public string Message { get; set; }
        public List<Face> Faces { get; set; }
        public List<TimekeepingPerson> TimekeepingPeople { get; set; }
        public List<IdentifyPersonRespone> IdentifyPeopleRespone { get; set; }

        public void SetError(string message)
        {
            HasError = true;
            Message = message;
        }
    }
}
