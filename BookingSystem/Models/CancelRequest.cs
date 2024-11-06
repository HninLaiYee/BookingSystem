namespace BookingSystem.Models
{
    public class CancelRequest
    {
        public string userId { get; set; }
        public string classScheduleId { get; set; }
        public string bookingId { get; set;}
    }
}
