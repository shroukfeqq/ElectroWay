using ElctroWay.Enums;
using ElctroWay.Models.Provider;
using System.ComponentModel.DataAnnotations;

namespace ElctroWay.Models.Identity
{
    public class ProviderProfile
    {
        [Key]
        public int ProviderId { get; set; }

            public int UserId { get; set; }

            public VerificationStatus VerificationStatus { get; set; }

            public DateTime? ReviewedAt { get; set; }

            public int? ReviewedBy { get; set; }

            public string? ReviewNotes { get; set; }

            public DateTime CreatedAt { get; set; }

            // Navigation
            public ApplicationUser? User { get; set; }

            public ApplicationUser? Reviewer { get; set; }

            public ICollection<ProviderDocument> Documents { get; set; } = new List<ProviderDocument>();

            public ICollection<Station> Stations { get; set; } = new List<Station>();
        
    }
}
