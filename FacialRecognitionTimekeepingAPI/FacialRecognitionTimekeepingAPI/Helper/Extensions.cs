using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Helper
{
    public static class Extensions
    {
        public static async Task<byte[]> GetBytesAsync(this Microsoft.AspNetCore.Http.IFormFile formFile)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
