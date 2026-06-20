using ElctroWay.Enums;
using ElctroWay.Models.booking;

namespace ElctroWay.Models.Provider
{
    public class Port
    {
        public int PortId { get; set; }

        public int StationId { get; set; }

        public int Power { get; set; }

        public ConnectorType ConnectorType { get; set; }

        public string PortCode { get; set; }

        public decimal PricePerKwh { get; set; }

        public PortStatus Status { get; set; }

        public string? QrData { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Station? Station { get; set; }

        public ICollection<PortImage> Images { get; set; } = new List<PortImage>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
