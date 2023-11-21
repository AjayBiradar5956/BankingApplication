using Microsoft.EntityFrameworkCore;

namespace BankingApplication.Models
{
    public class APIDbContext:DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasOne(u => u.BankAccount)
                .WithOne(b => b.Users)
                .HasForeignKey<BankAccount>(b => b.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
