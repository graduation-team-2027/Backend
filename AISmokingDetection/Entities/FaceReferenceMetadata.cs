
namespace AISmokingDetection.Entities
{
    public class FaceReferenceMetadata
    {
        public int FaceReferenceMetadataId { get; set; }

        public int ParticipantId { get; set; }

        public Participant Participant { get; set; } = null!;

        public string StorageKey { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
