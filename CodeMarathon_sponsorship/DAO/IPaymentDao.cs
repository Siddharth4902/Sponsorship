using CodeMarathon_sponsorship;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using CodeMarathon_sponsorship.Models;

namespace CodeMarathon_sponsorship.DAO
{
    public interface IPaymentDao
    {

        Task<int> InsertPayment(Payment p);
        Task<List<Payment>> GetPayments();
        Task<List<SponsorPaymentSummary>> GetSponsorPaymentSummaries();

        Task<List<MatchDetails>> GetMatchDetails();

        Task<List<SponsorMatchCount>> GetSponsorsAndMatchCounts(int year);

        Task<int> DeletePaymentById(int id);
    }
}
