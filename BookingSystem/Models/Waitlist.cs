using System;
using System.Collections.Generic;

namespace BookingSystem.Models
{
    public partial class Waitlist
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ClassScheduleId { get; set; }
        public DateTime? WaitlistTime { get; set; }

        public virtual ClassSchedule? ClassSchedule { get; set; }
    }
}
