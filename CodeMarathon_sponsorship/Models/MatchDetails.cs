using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CodeMarathon_sponsorship.Models
{
    public class MatchDetails
    {
        public int MatchId { get; set; }
        [Required (ErrorMessage = "Invalid match ID")]

        public DateTime? MatchDate { get; set; }

        public string Location { get; set; }
        public string MatchName { get; set; }

        public double TotalAmountOfPayments { get; set; }



    }

}
