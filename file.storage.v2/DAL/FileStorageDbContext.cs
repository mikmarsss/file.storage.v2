using file.storage.v2.Domain.Entities.Business;
using file_storage.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
namespace file_storage.DAL
{
    public class FileStorageDbContext: DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<FileEntity> UserFiles { get; set; }
        public DbSet<TokenEntity> Tokens { get; set; }

        public DbSet<LoggingEntity> Loggings { get; set; }
        public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>()
               .HasMany(u => u.Files)
               .WithOne(f => f.Owner);

            modelBuilder.Entity<UserEntity>()
               .HasOne(u => u.RefreshToken)
               .WithOne(f => f.User)
               .HasForeignKey<TokenEntity>(rt => rt.UserId);

            modelBuilder.Entity<UserEntity>()
              .HasMany(u => u.LoggedActions)
              .WithOne(f => f.User);
        }
    }
}
