using ElctroWay.Models;
using ElctroWay.Models.booking;
using ElctroWay.Models.OurSystem;
using ElctroWay.Models.vehicle;
using Microsoft.AspNetCore.Identity;

namespace ElctroWay.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }

        public bool IsBanned { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ProviderProfile? ProviderProfile { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
