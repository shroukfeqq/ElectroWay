using ElctroWay.Enums;
using ElctroWay.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElctroWay.Models.Payment
{
    public class WithdrawalRequest
    {
        [Key]
        public int WithdrawalId { get; set; }

        public int OwnerId { get; set; }

        public decimal Amount { get; set; }

        public WithdrawalMethod Method { get; set; }

        public string AccountDetails { get; set; }

        public WithdrawalStatus Status { get; set; }

        public int? ReviewedBy { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        // Navigation
        public ApplicationUser? Owner { get; set; }

        public ApplicationUser? Reviewer { get; set; }
    }
}
