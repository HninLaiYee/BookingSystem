using System;
using System.Collections.Generic;

namespace BookingSystem.Models
{
    public partial class Package
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public decimal? Price { get; set; }
        public int? Credits { get; set; }
        public bool? IsActive { get; set; }

        // Navigation property to related UserPackages
        public ICollection<UserPackage> UserPackages { get; set; }
    }
}
