using Microsoft.EntityFrameworkCore;
using mvc4.Models;

namespace mvc4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<RegistrationModel> Registration { get; set; }
        public DbSet<InvestorModel> InvestorTable { get; set; }
        public DbSet<FundModel> FundTable { get; set; } 
        public DbSet<ClientModel> ClientTable { get; set; }
    }
}
