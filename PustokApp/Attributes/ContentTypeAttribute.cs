using System.ComponentModel.DataAnnotations;

namespace PustokApp.Attributes
{
    public class ContentTypeAttribute : ValidationAttribute
    {
        private readonly string[] _allowedContentTypes;
        public ContentTypeAttribute(string[] allowedContentTypes)
        {
            _allowedContentTypes = allowedContentTypes;
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
                if (_allowedContentTypes.Contains(file.ContentType))
                {
                    return new ValidationResult($"File type must be one of the following: {string.Join(", ", _allowedContentTypes)}");
                }
            }
                return ValidationResult.Success;
        }
    }
}
