using Microsoft.EntityFrameworkCore;

namespace myAuthExampleApi.Models
{
    public partial class MyAuthExampleContext : DbContext
    {
        public MyAuthExampleContext()
        {
        }

        public MyAuthExampleContext(DbContextOptions<MyAuthExampleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public virtual DbSet<SimpleTokens> SimpleTokens { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Token });

                entity.Property(e => e.Token).HasMaxLength(40);
            });

            modelBuilder.Entity<SimpleTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Token });

                entity.Property(e => e.Token).HasMaxLength(40);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.UserName).HasMaxLength(128).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
