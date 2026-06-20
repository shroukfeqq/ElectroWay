using ElctroWay.Enums;
using ElctroWay.Models.Identity;

namespace ElctroWay.Models.OurSystem
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; }

        public string MessageBody { get; set; }

        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }
    }
}
