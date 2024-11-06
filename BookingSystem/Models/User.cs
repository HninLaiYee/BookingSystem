namespace BookingSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string CountryCode { get; set; }
        public int Credits { get; set; }

        public bool IsEmailVerified { get; set; } = false;

    }
}
