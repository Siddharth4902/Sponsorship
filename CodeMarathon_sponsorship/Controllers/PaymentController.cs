using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CodeMarathon_sponsorship.DAO;
using CodeMarathon_sponsorship.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CodeMarathon_sponsorship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentDao _paymentDao;

        public PaymentController(IPaymentDao paymentDao)
        {
            _paymentDao = paymentDao;
        }
       
        [HttpGet]
        public async Task<ActionResult<List<Payment>>> GetPayments()
        {
            var payments = await _paymentDao.GetPayments();
            if (payments == null)
            {
                return NotFound();
            }
            return Ok(payments);
        }



        [HttpPost]
        public async Task<ActionResult<int>> Payment(Payment payment)
        {
            if (payment != null)
            {
                if (ModelState.IsValid)
                {
                    int res = await _paymentDao.InsertPayment(payment);
                    if (res > 0)
                    {
                        return CreatedAtAction(nameof(GetPayments), new { id = payment.PaymentId }, payment);
                    }
                }
                return BadRequest("Database Failed to Add product");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("sponsor-sum")]
        public async Task<ActionResult<List<SponsorPaymentSummary>>> GetSponsorPaymentSummaries()
        {
            var summaries = await _paymentDao.GetSponsorPaymentSummaries();
            if (summaries == null || summaries.Count == 0)
            {
                return NotFound();
            }
            return Ok(summaries);
        }

        [HttpGet("Match-Details")]
        public async Task<ActionResult<List<MatchDetails>>> GetMatchDetails()
        {
            var Msummaries = await _paymentDao.GetMatchDetails();
            if (Msummaries == null || Msummaries.Count == 0)
            {
                return NotFound();
            }
            return Ok(Msummaries);
        }

        [HttpGet("sponsors match-counts")]
        public async Task<ActionResult<List<SponsorMatchCount>>> GetSponsorsAndMatchCounts([FromQuery] int year)
        {
            if (year <= 0)
            {
                return BadRequest("Invalid year parameter.");
            }

            var result = await _paymentDao.GetSponsorsAndMatchCounts(year);
            return Ok(result);
        }
    }
}


