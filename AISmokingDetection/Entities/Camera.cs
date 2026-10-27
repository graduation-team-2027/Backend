
namespace AISmokingDetection.Entities
{
    public class Camera
    {
        public int CameraId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CameraCode { get; set; } = string.Empty;

        public int LocationId { get; set; }

        public Location Location { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
