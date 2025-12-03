using file_storage.Domain.Entities.Base;
using file_storage.Domain.Entities.Business;

namespace file.storage.v2.Domain.Entities.Business
{
    /// <summary>
    /// Публичный класс сущности Токен
    /// </summary>
    public class TokenEntity : EntityBase
    {
        /// <summary>
        /// Токен сущности Токен
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// ID пользователя сущности Пользователь
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Связь сущности пользователь 
        /// для сущности Токен
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        /// Дата окончания действия refresh токена 
        /// для сущности Токен
        /// </summary>
        public DateTime TokenExpires { get; set; }

        /// <summary>
        /// Публичный контроллер сущности Токен
        /// </summary>
        public TokenEntity(string Token, UserEntity User, DateTime expires) 
        {
            this.Token = Token;
            this.User = User;
            this.TokenExpires = expires;
        }

        /// <summary>
        /// Контроллер для EF Core
        /// </summary>
        public TokenEntity() { }
    }
}
