using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.HelperClasses
{
    public class UploadHelper
    {
        public byte[] ConvertToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
