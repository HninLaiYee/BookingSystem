namespace BookingSystem.Models
{
    public class LoginTokenModel
    {
        public string userid { get; set; }

        public string username { get; set; }    
        public string access_token { get; set; }

        public string expires_in { get; set; }
    }
}
