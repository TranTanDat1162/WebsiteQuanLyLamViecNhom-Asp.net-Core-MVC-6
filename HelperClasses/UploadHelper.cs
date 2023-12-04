using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.HelperClasses
{
    public class UploadHelper
    {
        /// <summary>
        /// Get data stream from IFormFile files for upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
