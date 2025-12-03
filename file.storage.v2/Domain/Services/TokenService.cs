using file.storage.v2.Domain.Entities.Business;
using file.storage.v2.Domain.Interfaces;
using file_storage.Domain.Entities.Business;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace file.storage.v2.Domain.Services
{
    /// <summary>
    /// Публичный класс сервиса создания и валидации Токенов
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Функция создания access Токена
        /// </summary>
        public TokenResult GenerateAccessToken(UserEntity user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var accessTokenExpiresMinutes = Convert.ToInt32(_configuration["Jwt:AccessTokenExpiresInMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("token_type", "access")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(accessTokenExpiresMinutes),
                signingCredentials: credentials
            );
             return new TokenResult
             {
                 AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                 Expires = token.ValidTo,
             };
        }

        /// <summary>
        /// Функция создания refresh Токена
        /// </summary>
        public TokenEntity GenerateRefreshToken(UserEntity user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var refreshTokenExpiresDays = Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiresInDays"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("token_type", "refresh")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(refreshTokenExpiresDays),
                signingCredentials: credentials
            );

            TokenEntity refreshToken = new TokenEntity(new JwtSecurityTokenHandler().WriteToken(token), user, token.ValidTo);

            return refreshToken;
        }

        /// <summary>
        /// Функция создания access и refresh Токена
        /// </summary>
        public (TokenResult accessToken, TokenEntity refreshToken) GenerateTokens(UserEntity user)
        {
            return (
                GenerateAccessToken(user),
                GenerateRefreshToken(user)
            );
        }
    }
}
