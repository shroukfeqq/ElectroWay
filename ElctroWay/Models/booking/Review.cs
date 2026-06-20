using ElctroWay.Models.Identity;

namespace ElctroWay.Models.booking
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int UserId { get; set; }

        public int SessionId { get; set; }

        public int Rate { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }

        public ChargingSession? Session { get; set; }
    }
}
