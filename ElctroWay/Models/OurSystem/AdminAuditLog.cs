using ElctroWay.Enums;
using ElctroWay.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElctroWay.Models.OurSystem
{
    public class AdminAuditLog
    {
        [Key]
        public int LogId { get; set; }

        public int AdminId { get; set; }

        public string Action { get; set; }

        public string EntityType { get; set; }

        public int EntityId { get; set; }

        public string? Details { get; set; }

        public AuditStatus Status { get; set; }

        public string? IpAddress { get; set; }

        public DateTime PerformedAt { get; set; }

        // Navigation
        public ApplicationUser? Admin { get; set; }
    }
}
