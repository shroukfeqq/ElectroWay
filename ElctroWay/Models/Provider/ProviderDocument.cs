using ElctroWay.Enums;
using ElctroWay.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ElctroWay.Models.Provider
{
    public class ProviderDocument
    {
        [Key]
        public int DocumentId { get; set; }

        public int ProviderId { get; set; }

        public string ImageUrl { get; set; }

        public DocumentType DocumentType { get; set; }

        public DocumentStatus Status { get; set; }

        public DateTime UploadedAt { get; set; }

        // Navigation
        public ProviderProfile? Provider { get; set; }
    }
}
