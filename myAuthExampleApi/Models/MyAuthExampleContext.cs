using Microsoft.EntityFrameworkCore;

#nullable disable

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
                entity.HasIndex(e => e.UserName, "IX_UserName")
                    .IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
