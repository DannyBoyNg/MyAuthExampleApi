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

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<SimpleToken> SimpleTokens { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Token });

                entity.Property(e => e.Token).HasMaxLength(40);
            });

            modelBuilder.Entity<SimpleToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Token });

                entity.Property(e => e.Token).HasMaxLength(40);
            });

            modelBuilder.Entity<User>(entity =>
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
