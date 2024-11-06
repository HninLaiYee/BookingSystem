using System;
using System.Collections.Generic;

namespace BookingSystem.Models
{
    public partial class UserBooking
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ClassScheduleId { get; set; }
        public DateTime? BookingTime { get; set; }
        public bool? IsCancelled { get; set; }

        public virtual ClassSchedule? ClassSchedule { get; set; }
    }
}
