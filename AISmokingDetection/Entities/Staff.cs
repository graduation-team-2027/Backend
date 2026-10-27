namespace AISmokingDetection.Entities
{
    public class Staff
    {
        public int StaffId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Reviewer";

        public bool IsActive { get; set; } = true;
    }
}
