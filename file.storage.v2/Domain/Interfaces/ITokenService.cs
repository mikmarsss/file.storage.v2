using file.storage.v2.Domain.Entities.Business;
using file_storage.Domain.Entities.Business;

namespace file.storage.v2.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс реализации сервиса создания и валидации Токенов
    /// </summary>
    public interface ITokenService
    {
        TokenResult GenerateAccessToken(UserEntity user);
        TokenEntity GenerateRefreshToken(UserEntity user);
        (TokenResult accessToken, TokenEntity refreshToken) GenerateTokens(UserEntity user);
    }

    public class TokenResult
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
    }
}
