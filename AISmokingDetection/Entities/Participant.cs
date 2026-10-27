
namespace AISmokingDetection.Entities
{
    public class Participant
    {
        public int ParticipantId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string ParticipantCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<FaceReferenceMetadata> FaceReferenceMetadata { get; set; } = new List<FaceReferenceMetadata>();
    }
}
