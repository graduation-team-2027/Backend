
using AISmokingDetection.Entities;
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
    }
}
