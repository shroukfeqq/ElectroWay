using ElctroWay.Models.Identity;
using ElctroWay.Models.Provider;

namespace ElctroWay.Models.booking
{
    public class Favorite
    {
        public int FavoriteId { get; set; }

        public int UserId { get; set; }

        public int StationId { get; set; }

        public DateTime SavedAt { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }

        public Station? Station { get; set; }
    }
}
