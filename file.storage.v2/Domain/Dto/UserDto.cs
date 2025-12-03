using file_storage.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;

namespace file_storage.Domain.Dto
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email обязательно")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password обязательно")]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
        public string? Role { get; set; }
        public List<FileEntity>? Files { get; set; }
    }
}
