namespace AISmokingDetection.Entities
{
    public class GalleryBuild
    {
        public int GalleryBuildId { get; set; }

        public long Version { get; set; }

        public GalleryBuildStatus Status { get; set; } = GalleryBuildStatus.Queued;

        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAtUtc { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
