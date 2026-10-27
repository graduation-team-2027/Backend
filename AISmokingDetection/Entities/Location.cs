
namespace AISmokingDetection.Entities
{
    public class Location
    {
        public int LocationId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Camera> Cameras { get; set; } = new List<Camera>();
    }
}
