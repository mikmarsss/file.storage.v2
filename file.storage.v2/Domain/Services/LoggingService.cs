using file.storage.v2.Domain.Entities.Business;
using file.storage.v2.Domain.Interfaces;
using file_storage.DAL;
using file_storage.Domain.Entities.Business;

namespace file.storage.v2.Domain.Services
{
    public class LoggingService: ILoggingService
    {
        private readonly IConfiguration _configuration;
        private readonly FileStorageDbContext _context;
        public LoggingService(IConfiguration configuration, FileStorageDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public void WriteLogging( UserEntity User, string method) 
        {
            var logging = new LoggingEntity
            {
                User = User,
                UserName = User.Name,
                Action = method,
            };

            _context.Loggings.Add(logging);
            _context.SaveChanges();
        }
    }
}
