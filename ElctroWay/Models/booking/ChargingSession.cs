using ElctroWay.Enums;
using ElctroWay.Models.Payment;
using System.ComponentModel.DataAnnotations;


namespace ElctroWay.Models.booking
{
    public class ChargingSession
    {
        [Key]
        public int SessionId { get; set; }

        public int BookingId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public decimal EnergyKwh { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PlatformFee { get; set; }

        public decimal OwnerProfit { get; set; }

        public SessionStatus Status { get; set; }

        // Navigation
        public Booking? Booking { get; set; }

        public Transaction? Transaction { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
