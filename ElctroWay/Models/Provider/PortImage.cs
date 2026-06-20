using System.ComponentModel.DataAnnotations;

namespace ElctroWay.Models.Provider
{
    public class PortImage
    {
        [Key]
        public int ImageId { get; set; }

        public int PortId { get; set; }

        public string ImageUrl { get; set; }

        public string ImageType { get; set; }

        public int SortOrder { get; set; }

        public DateTime UploadedAt { get; set; }

        public Port? Port { get; set; }
    }
}
