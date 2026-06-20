namespace ElctroWay.Models.Provider
{
    public class StationImage
    {
        public int StationImageId { get; set; }

        public int StationId { get; set; }

        public string ImageUrl { get; set; }

        public string ImageType { get; set; }

        public Station Station { get; set; }
    }
}
