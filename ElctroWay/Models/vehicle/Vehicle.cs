using ElctroWay.Enums;
using ElctroWay.Models.Identity;

namespace ElctroWay.Models.vehicle
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public int UserId { get; set; }

        public string VehicleName { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public decimal BatteryCapacity { get; set; }

        public ConnectorType ConnectorType { get; set; }

        public decimal ConsumptionRate { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }
    }
}
