using file_storage.Domain.Dto;
using file_storage.Domain.Entities.Base;

namespace file_storage.Domain.Entities.Business
{
    /// <summary>
    /// Публичный класс сущности Файл
    /// </summary>
    public class FileEntity: EntityBase
    {
        /// <summary>
        /// Имя сущности Файл
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Название файлы в системе для сущности Файл
        /// </summary>
        public string FileName { get;  set; }

        /// <summary>
        /// Название файлы в системе для сущности Файл
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Размер файлы в системе для сущности Файл
        /// </summary>
        public double Size { get;  set; }

        /// <summary>
        /// Связь с сущностью Пользователь 
        /// для сущности Файл
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Связь с сущностью Пользователь 
        /// для сущности Файл
        /// </summary>
        public UserEntity Owner { get;  set; }

        /// <summary>
        /// Конструктор для EF Core
        /// для сущности Файл
        /// </summary>
        public FileEntity() { }

        /// <summary>
        /// Метод для преобразования сущности Пользователь
        /// в DTO 
        /// </summary
        public FileDto ToDto()
        {
            return new FileDto
            {
                Id = this.Id,
                Name = this.Name,
                File = this.FileName,
                Size = this.Size,
                ContentType = this.ContentType,
                Owner = this.Owner.ToDto(),
            };
        }
    }
}
