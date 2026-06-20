namespace ElctroWay.Models.Payment
{
    public class Receipt
    {
        public int ReceiptId { get; set; }

        public int TransactionId { get; set; }

        public string ReceiptCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal EnergyKwh { get; set; }

        public decimal PricePerKwh { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime IssuedAt { get; set; }

        // Navigation
        public Transaction? Transaction { get; set; }
    }
}
