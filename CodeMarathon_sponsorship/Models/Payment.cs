using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CodeMarathon_sponsorship.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        [Required(ErrorMessage = "Provide a PayemntID")]

        public int ContractID { get; set; }

        public string PaymentDate { get; set; }
        [Required(ErrorMessage = "Provide PaymentDate")]
        public double AmountPaid { get; set; }
        [Required(ErrorMessage = "Provide AmountPaid")]
        public string PaymentStatus { get; set; }
 

    }

}
