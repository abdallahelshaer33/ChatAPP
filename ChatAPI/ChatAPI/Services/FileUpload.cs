

namespace ChatAPI.Services
{
    public class FileUpload
    {
        public static async Task<string> Upload(IFormFile file)
        {
            var uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" , 
                "upload");
            if (!Directory.Exists(uploadFile))
            {
                Directory.CreateDirectory(uploadFile);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filepath = Path.Combine(uploadFile, filename);
            await using var stream = new FileStream(filepath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filename;
        }
    }
}
