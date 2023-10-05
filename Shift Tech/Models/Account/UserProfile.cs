namespace Shift_Tech.Models.Account
{
    public class UserProfile
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsSeller { get; set; }
        public bool IsAdmin { get; set; }
    }
}
