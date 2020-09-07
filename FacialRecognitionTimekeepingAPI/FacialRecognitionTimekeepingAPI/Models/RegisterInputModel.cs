using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Models
{
    public class RegisterInputModel
    {
        public PersonInfo PersonInfo { get; set; }
        public byte[] FaceData { get; set; }
        public IFormFile FormFile { get; set; }
        public string AliasId { get; set; }
    }
}
