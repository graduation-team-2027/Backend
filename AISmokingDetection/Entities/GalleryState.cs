namespace AISmokingDetection.Entities
{
    public class GalleryState
    {
        public int GalleryStateId { get; set; } = 1;

        public long DesiredVersion { get; set; } = 0;

        public long? AppliedVersion { get; set; }

        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastBuildCompletedAtUtc { get; set; }

        public byte[] RowVersion { get; set; } = [];
    }
}
