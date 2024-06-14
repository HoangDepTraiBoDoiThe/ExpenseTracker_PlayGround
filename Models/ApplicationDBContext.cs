using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}