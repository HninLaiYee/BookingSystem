using System;
using System.Collections.Generic;

namespace BookingSystem.Models
{
    public partial class ClassSchedule
    {
        public ClassSchedule()
        {
            UserBookings = new HashSet<UserBooking>();
            Waitlists = new HashSet<Waitlist>();
        }

        public Guid Id { get; set; }

        public Guid packageId { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxParticipants { get; set; }
        public int? CurrentParticipants { get; set; }

        public virtual ICollection<UserBooking> UserBookings { get; set; }
        public virtual ICollection<Waitlist> Waitlists { get; set; }
    }
}
