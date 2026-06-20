using ElctroWay.Enums;
using ElctroWay.Models.booking;

using ElctroWay.Models.Identity;

namespace ElctroWay.Models.Provider
{
    public class Station
    {
        public int StationId { get; set; }

        public int ProviderId { get; set; }

        public string DisplayName { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? Description { get; set; }

        public StationStatus Status { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedBy { get; set; }

        public string? ReviewNotes { get; set; }

        // Navigation
        public ProviderProfile? Provider { get; set; }

        public ApplicationUser? Reviewer { get; set; }

        public ICollection<Port> Ports { get; set; } = new List<Port>();

        public ICollection<StationImage> Images { get; set; } = new List<StationImage>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
