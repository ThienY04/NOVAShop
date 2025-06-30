namespace lab03.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
