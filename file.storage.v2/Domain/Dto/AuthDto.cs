using file.storage.v2.Domain.Interfaces;
using file_storage.Domain.Dto;

namespace file.storage.v2.Domain.Dto
{
    public class AuthDto
    {
        public required UserDto User { get; set; }
        public required TokenResult AccessToken { get; set; }
    }
}
