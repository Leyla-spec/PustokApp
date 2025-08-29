using System.ComponentModel.DataAnnotations;

namespace PustokApp.Attributes
{
    public class FileLengthAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInMb;
        public FileLengthAttribute(int maxFileSizeInMb)
        {
            _maxFileSizeInMb = maxFileSizeInMb *1024 *1024;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = new List<IFormFile>();
            var files = value as IFormFile;
            if (files != null)
                list.Add(files);
            var fileList = value as List<IFormFile>;
            if (fileList != null)
                list.AddRange(fileList);
            foreach (var file in list)
            {
                if (file.Length > _maxFileSizeInMb)
                {
                    return new ValidationResult($"File size must be less than {_maxFileSizeInMb / (1024 * 1024)} MB");
                }
            }

            return ValidationResult.Success;
        }
    }
}
