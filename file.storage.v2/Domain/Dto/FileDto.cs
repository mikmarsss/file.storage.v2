using file_storage.Domain.Entities.Business;

namespace file_storage.Domain.Dto
{
    public class FileDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? File { get;  set; }
        public string? ContentType { get; set; }
        public double? Size { get; set; }
        public UserDto? Owner { get;  set; }
    }
}
