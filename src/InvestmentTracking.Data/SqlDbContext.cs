using InvestmentTracking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracking.Data;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
    }

    public DbSet<Broker> Brokers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }
    public DbSet<AccountTransaction> AccountTransactions { get; set; }
    public DbSet<IncomeTransaction> IncomeTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Commission)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>().HasDiscriminator<string>("TransactionType")
            .HasValue<AccountTransaction>("Account")
            .HasValue<StockTransaction>("Stock")
            .HasValue<IncomeTransaction>("Income");
    }

    public void ApplyMigrations()
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }
}