using System.Globalization;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;

namespace ExpenseTracker.Controllers
{
    public class Dashboard : Controller
    {
        private readonly ApplicationDbContext _context;

        public Dashboard(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: Dashboard
        public async Task<ActionResult> Index()
        {
            DateTime startDate = DateTime.Today.AddDays(-6); 
            DateTime endDate = DateTime.Today;

            List<Transaction> selectedTransaction = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= startDate)
                .ToListAsync();

            int totalIncome = selectedTransaction.Where(transaction => transaction.Category?.Type == "Income")
                .Sum(x => x.Amount);
            int totalExpense = selectedTransaction.Where(transaction => transaction.Category?.Type == "Expense")
                .Sum(x => x.Amount);
            int totalAmount = totalIncome - totalExpense;

            ViewBag.TotalIncome = String.Format(CultureInfo.GetCultureInfo("es-US"), "{0:c2}", totalIncome);
            ViewBag.TotalExpense = String.Format(CultureInfo.GetCultureInfo("es-US"), "{0:c2}", totalExpense);
            
            CultureInfo balanceFormat = CultureInfo.CreateSpecificCulture("en-US");
            balanceFormat.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(balanceFormat, "{0:c2}", totalAmount);

            ViewBag.ExpenseChart = selectedTransaction
                .Where(x => x.Category?.Type == "Expense")
                .GroupBy(g => g.CategoryId)
                .Select(s =>
                    new
                    {
                        categoryWithIcon = s.First().Category?.TitleWithIcon,
                        amount = s.Sum(su => su.Amount),
                        formattedAmount = string.Format(balanceFormat, "{0:c2}", s.Sum(su => su.Amount))
                    }
                ).ToList();
            
            return View();
        }

    }
}
