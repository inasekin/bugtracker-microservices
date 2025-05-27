using Microsoft.EntityFrameworkCore;
using VideoCallService.Domain.Models;

namespace VideoCallService.DAL
{
    public class VideoCallDbContext : DbContext
    {
        public VideoCallDbContext(DbContextOptions<VideoCallDbContext> options) : base(options)
        {
        }

        public DbSet<VideoRoom> VideoRooms { get; set; }
        public DbSet<VideoParticipant> VideoParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация VideoRoom
            modelBuilder.Entity<VideoRoom>(entity =>
            {
                entity.ToTable("video_rooms"); // Используем snake_case для PostgreSQL
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccessCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                
                // Связь один-ко-многим с участниками
                entity.HasMany(e => e.Participants)
                      .WithOne()
                      .HasForeignKey(e => e.RoomId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация VideoParticipant
            modelBuilder.Entity<VideoParticipant>(entity =>
            {
                entity.ToTable("video_participants"); // Используем snake_case для PostgreSQL
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.JoinedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
} 