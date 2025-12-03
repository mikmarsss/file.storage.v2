using file_storage.Domain.Entities.Business;

namespace file.storage.v2.Domain.Interfaces
{
    public interface ILoggingService
    {
        void WriteLogging(UserEntity User, string method);
    }
}
