using file.storage.v2.Domain.Dto;
using file.storage.v2.Domain.Entities.Business;
using file.storage.v2.Domain.Interfaces;
using file_storage.Domain.Constants;
using file_storage.Domain.Dto;
using file_storage.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace file_storage.Domain.Entities.Business
{
    /// <summary>
    /// Публичный класс сущности Пользователь
    /// </summary>
    public class UserEntity : EntityBase
    {
        /// <summary>
        /// Имя сущности Пользователь
        /// </summary>
        public string Name { get;  set; }

        /// <summary>
        /// Почта сущности Пользователь
        /// </summary
        public string Email { get;  set; }

        /// <summary>
        /// Пароль сущности Пользователь
        /// </summary
        public string Password { get;  set; }

        /// <summary>
        /// Роль сущности Пользователь
        /// </summary
        public string? Role { get;  set; }

        /// <summary>
        /// Связь сущности Пользователь
        /// с сущностью Файлы Пользователя
        /// </summary
        public List<FileEntity>? Files { get; private set; }

        /// <summary>
        /// Связь сущности Пользователь
        /// с сущностью Refresh токен Пользователя
        /// </summary
        public TokenEntity RefreshToken { get; private set; }

        /// <summary>
        /// Связь сущности Пользователь
        /// с сущностью Логирования
        /// </summary
        public List<LoggingEntity> LoggedActions { get; private set; }

        /// <summary>
        /// Публичный конструктор сущности Пользователь
        /// </summary
        public UserEntity(string name, string email, string password, List<FileEntity> files)
        {
            this.Name = name;
            this.Email = email;
            this.Password = password;
            this.Role = UserRoles.User;
            this.Files = files;
        }

        /// <summary>
        /// Конструктор для EF Core
        /// </summary
        public UserEntity() { }

        /// <summary>
        /// Функция для создания новой сущности Пользователь
        /// </summary
        public static UserEntity CreateNewUser(string name, string email, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return new UserEntity(name, email, hashedPassword, new List<FileEntity>());
        }

        /// <summary>
        /// Метод для обновления сущности Пользователь
        /// </summary
        public void UpdateProfile(string name, string email)
        {
            Name = name;
            Email = email;
        }

        /// <summary>
        /// Метод для преобразования сущности Пользователь
        /// в DTO 
        /// </summary
        public UserDto ToDto()
        {
            return new UserDto
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Role = this.Role,
                Files = this.Files,
            };
        }

        public AuthDto ToAuthUserDto(TokenResult accessToken)
        {
            return new AuthDto
            {
                User = new UserDto
                {
                    Id = this.Id,
                    Name = this.Name,
                    Email = this.Email,
                    Role = this.Role,
                    Files = this.Files,
                },
                AccessToken = accessToken
            };
        }
    }
}
