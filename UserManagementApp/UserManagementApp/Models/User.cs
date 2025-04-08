namespace UserManagementApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime RegistrationTime { get; set; } = DateTime.Now;
    }
}