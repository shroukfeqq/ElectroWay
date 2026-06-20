using ElctroWay.Enums;
using ElctroWay.Models.Identity;
using ElctroWay.Models.Provider;

namespace ElctroWay.Models.booking
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int PortId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ScheduledAt { get; set; }

        public BookingStatus Status { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? ReasonOfCancellation { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        

        // Navigation
        public ApplicationUser? User { get; set; }

        public Port? Port { get; set; }

        public ChargingSession? ChargingSession { get; set; }
    }
}
