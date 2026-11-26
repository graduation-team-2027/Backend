using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Data;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Business.Services
{
    public class GalleryVersionService : IGalleryVersionService
    {
        private readonly AppDbContext _context;

        public GalleryVersionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task IncrementDesiredVersionAsync(
            CancellationToken cancellationToken)
        {
            var updatedAt = DateTime.UtcNow;

            var affectedRows = await _context.GalleryStates
                .Where(g => g.GalleryStateId == 1)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            g => g.DesiredVersion,
                            g => g.DesiredVersion + 1)
                        .SetProperty(
                            g => g.LastUpdatedAtUtc,
                            updatedAt),
                    cancellationToken);

            if (affectedRows != 1)
            {
                throw new InvalidOperationException(
                    "GalleryState singleton is missing. Apply the Gallery Versioning migration before changing eligible face references.");
            }
        }
    }
}
