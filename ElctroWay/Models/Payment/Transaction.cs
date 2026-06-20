using ElctroWay.Enums;
using ElctroWay.Models.booking;

namespace ElctroWay.Models.Payment
{
    public class Transaction
    {
       

        public int TransactionId { get; set; }

        public int SessionId { get; set; }

        public string TxCode { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public TransactionStatus Status { get; set; }
       

        public DateTime? PaidAt { get; set; }

        // Navigation

        public ChargingSession? Session { get; set; }
        public Receipt? Receipt { get; set; }
    }
}
