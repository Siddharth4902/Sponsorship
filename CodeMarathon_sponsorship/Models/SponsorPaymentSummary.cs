using System;

namespace CodeMarathon_sponsorship.Models
{
    public class SponsorPaymentSummary
    {
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }
        public double TotalPayments { get; set; }
        public int NumberOfPayments { get; set; }
        public DateTime? LatestPaymentDate { get; set; }
    }
}

