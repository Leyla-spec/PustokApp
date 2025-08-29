namespace PustokApp.Extension
{
    public static class FileManager
    {
        public static string SaveFile(this IFormFile file, string folderPath)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", folderPath, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
                file.CopyTo(stream);
            return fileName;
        }
        public static bool CheckFileSize(this IFormFile file, int mb)
        {
            
            return file.Length <= mb * 1024 * 1024;
        }
        public static bool CheckContentType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
        public static void DeleteFile(string folderPath, string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
