
using AISmokingDetection.Entities;
using AISmokingDetection.Model;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<FaceReferenceMetadata> FaceReferenceMetadata { get; set; }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
        public DbSet<ReviewHistory> ReviewHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Incident>()
    .Property(i => i.ConfidenceScore)
    .HasPrecision(5, 4);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Camera)
                .WithMany()
                .HasForeignKey(i => i.CameraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Participant)
                .WithMany()
                .HasForeignKey(i => i.ParticipantId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Evidence>()
                .HasOne(e => e.Incident)
                .WithMany(i => i.Evidences)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProcessingJob>()
                .HasOne(p => p.Incident)
                .WithMany(i => i.ProcessingJobs)
                .HasForeignKey(p => p.IncidentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ReviewHistory>()
                .HasOne(r => r.Incident)
                .WithMany(i => i.ReviewHistories)
                .HasForeignKey(r => r.IncidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewHistory>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
