using CodeMarathon_sponsorship.DAO;
using Npgsql;
namespace CodeMarathon_sponsorship
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("PostgreDB");
            builder.Services.AddScoped((provider) => new NpgsqlConnection(connectionString));
            builder.Services.AddScoped<IPaymentDao, ProductDaoImplementation>();
            // Add services to the container.

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
                options.AddPolicy("FrontEndClient", builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000/"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAny");
            app.UseCors("FrontEndClient");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
