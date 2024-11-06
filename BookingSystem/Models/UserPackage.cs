using System;
using System.Collections.Generic;

namespace BookingSystem.Models
{
    public partial class UserPackage
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PackageId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? RemainingCredits { get; set; }
        public bool IsExpired => DateTime.Now > ExpirationDate;

        public Package Package { get; set; }
    }
}
