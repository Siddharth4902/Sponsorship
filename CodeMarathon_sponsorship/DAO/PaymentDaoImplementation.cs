using CodeMarathon_sponsorship;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using CodeMarathon_sponsorship.Models;
using Microsoft.AspNetCore.Mvc;
using CodeMarathon_sponsorship.DAO;

namespace CodeMarathon_sponsorship.DAO
{
    public class ProductDaoImplementation : IPaymentDao
    {
        NpgsqlConnection _connection;
        public ProductDaoImplementation(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Payment>> GetPayments()
        {
            List<Payment?> paymentList = new List<Payment?>();
            Payment payment = null;
            string errorMessage = string.Empty;
            string query = @"select * from sponsor.Payments order by PaymentId desc";
            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand(query, _connection);
                    command.CommandType = CommandType.Text;
            
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if ((reader.HasRows))
                    {
                        while (reader.Read())
                        {
                            payment = new Payment();
                            payment.PaymentId = reader.GetInt32(0);
                            payment.ContractID = reader.GetInt32(1); 
                            payment.PaymentDate = reader.GetDateTime(2).ToString("yyyy-MM-dd"); // Convert DateTime to string
                            payment.AmountPaid = reader.GetDouble(3);
                            payment.PaymentStatus = reader.GetString(4);

                            paymentList.Add(payment);
                        }

                    }
                    reader?.Close();

                }
            }
            catch (NpgsqlException e)
            {
                errorMessage = e.Message;
                Console.WriteLine("----Exception -----: message");
            }
            return paymentList;
        }

        public async Task<int> InsertPayment(Payment p)
        {
            int rowsInserted = 0;
            string message;
            string insertQuery = $"insert into sponsor.Payments(ContractID, PaymentDate, AmountPaid, PaymentStatus) values('{p.ContractID}','{p.PaymentDate}','{p.AmountPaid}','{p.PaymentStatus}')";
            Console.WriteLine("Query" + insertQuery);
            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, _connection);
                    insertCommand.CommandType = CommandType.Text;
                    rowsInserted = await insertCommand.ExecuteNonQueryAsync();
                }

            }
            catch (NpgsqlException e)
            {
                message = e.Message;
                Console.WriteLine("---------Exception----------" + message);
            }
            return rowsInserted;
        }

        public async Task<List<SponsorPaymentSummary>> GetSponsorPaymentSummaries()
        {
            List<SponsorPaymentSummary> summaries = new List<SponsorPaymentSummary>();
            string query = @"
        select s.SponsorId,s.SponsorName,
coalesce(SUM(p.AmountPaid), 0) as TotalPayments,
count(p.PaymentId) AS NumberOfPayments,
max(p.PaymentDate) AS LatestPaymentDate
from Sponsor.Sponsors s
left join Sponsor.Contracts c on s.SponsorId = c.SponsorId
left join Sponsor.Payments p on c.ContractId = p.ContractId
group by s.SponsorId, s.SponsorName 
order by s.SponsorId;
    ";

            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    using (var command = new NpgsqlCommand(query, _connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                summaries.Add(new SponsorPaymentSummary
                                {
                                    SponsorId = reader.GetInt32(0),
                                    SponsorName = reader.GetString(1),
                                    TotalPayments = reader.GetDouble(2),
                                    NumberOfPayments = reader.GetInt32(3),
                                    LatestPaymentDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return summaries;


        }


       
        public async Task<List<MatchDetails>> GetMatchDetails()
        {
            List<MatchDetails> Msummaries = new List<MatchDetails>();
            string query = @"
            select m.matchid as MatchId,m.MatchName,m.MatchDate,m.Location,
coalesce(SUM(p.AmountPaid),0) as TotalAmountOfPayments
from Sponsor.Matches m
left join Sponsor.Contracts c on m.MatchId=c.MatchId
left join Sponsor.Payments p on c.ContractId=p.ContractId
group by m.matchid,m.MatchDate,m.Location
order by m.Matchid;
    ";

            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    using (var command = new NpgsqlCommand(query, _connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Msummaries.Add(new MatchDetails
                                {
                                    MatchId = reader.GetInt32(0),
                                    MatchName = reader.GetString(1),
                                    MatchDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                                    Location = reader.GetString(3),
                                    TotalAmountOfPayments = reader.GetDouble(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return Msummaries;
        }

        public async Task<List<SponsorMatchCount>> GetSponsorsAndMatchCounts(int year)
        {
            var sponsorMatchCounts = new List<SponsorMatchCount>();
            string query = @"
                select s.SponsorId, s.SponsorName, count(m.MatchId) as NumberOfMatches
from Sponsor.Sponsors s
full join Sponsor.Contracts c ON s.SponsorId = c.SponsorId 
full join Sponsor.Matches m ON c.MatchId = m.MatchId
where extract(YEAR from m.MatchDate) = @Year
group by s.SponsorId, s.SponsorName;
";

            try
            {
                await _connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    // Define the parameter and add it to the command
                    command.Parameters.AddWithValue("Year", year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var sponsorMatchCount = new SponsorMatchCount
                            {
                                SponsorId = reader.GetInt32(0),
                                SponsorName = reader.GetString(1),
                                NumberOfMatches = reader.GetInt32(2)
                            };
                            sponsorMatchCounts.Add(sponsorMatchCount);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return sponsorMatchCounts;
        }

        public async Task<int> DeletePaymentById(int id)
        {
            int rowAffected = 0;
            string delQuery = $"delete from sponsor.Payments where PaymentId =@pid"; ;
            Console.WriteLine("query" + delQuery);
            // conn.Open();
            try
            {
                using (_connection)
                {
                    await _connection.OpenAsync();
                    NpgsqlCommand deleteCommand = new NpgsqlCommand(delQuery, _connection);
                    deleteCommand.CommandType = CommandType.Text;
                    //updateCommand.Parameters.AddWithValue("@pid", id);
                    deleteCommand.Parameters.Add("@pid", NpgsqlDbType.Integer).Value = id;
                    rowAffected = await deleteCommand.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }


            return rowAffected;

        }

    }
}


    
