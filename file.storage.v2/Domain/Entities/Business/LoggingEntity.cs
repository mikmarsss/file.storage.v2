using file_storage.Domain.Entities.Base;
using file_storage.Domain.Entities.Business;

namespace file.storage.v2.Domain.Entities.Business
{
    public class LoggingEntity: EntityBase
    {
        /// <summary>
        /// Имя действия пользователя
        /// </summary
        public string Action {  get; set; }

        /// <summary>
        /// Связь сущностью Пользователь
        /// </summary
        public UserEntity User { get; set; }

        /// <summary>
        /// ID пользователя, совершившего дейсвтие
        /// </summary
        public string UserName { get; set; }

        /// <summary>
        /// Конструктор для EF Core
        /// </summary
        public LoggingEntity() { }
    }
}
