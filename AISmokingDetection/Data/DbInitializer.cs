using AISmokingDetection.Entities;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var location = await context.Locations
                .FirstOrDefaultAsync(x => x.Name == "Demo Location");

            if (location == null)
            {
                location = new Location
                {
                    Name = "Demo Location",
                    Address = "Demo Address - Test Data",
                    IsActive = true
                };

                context.Locations.Add(location);
                await context.SaveChangesAsync();
            }

            var participant = await context.Participants
                .FirstOrDefaultAsync(x => x.ParticipantCode == "DEMO-P001");

            if (participant == null)
            {
                participant = new Participant
                {
                    FullName = "Demo Participant",
                    ParticipantCode = "DEMO-P001",
                    IsActive = true
                };

                context.Participants.Add(participant);
                await context.SaveChangesAsync();
            }

            var cameraExists = await context.Cameras
                .AnyAsync(x => x.CameraCode == "DEMO-C001");

            if (!cameraExists)
            {
                context.Cameras.Add(new Camera
                {
                    Name = "Demo Camera",
                    CameraCode = "DEMO-C001",
                    LocationId = location.LocationId,
                    IsActive = true
                });

                await context.SaveChangesAsync();
            }
        }
    }
}