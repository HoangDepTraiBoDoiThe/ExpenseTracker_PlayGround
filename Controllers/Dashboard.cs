using System.Globalization;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            ViewBag.DonutChart = selectedTransaction
                .GroupBy(g => g.CategoryId)
                .Select(s =>
                    new
                    {
                        categoryWithIcon = s.First().Category?.TitleWithIcon,
                        amount = s.Sum(su => su.Amount),
                        formattedAmount = string.Format(balanceFormat, (s.First().Category?.Type == "Expense" ? "-" : "" ) + "{0:c2}", s.Sum(su => su.Amount))
                    }
                ).ToList();

            var expenseGr = selectedTransaction.Where(w => w.Category?.Type == "Expense")
                .GroupBy(g => g.Date)
                .Select(s => new SplineChartType()
                {
                    Day = s.First().Date.ToString("dd/MM"),
                    TotalExpense = s.Sum(su => su.Amount)
                });

            var incomeGr = from transaction in selectedTransaction
                where transaction.Category?.Type == "Income"
                group transaction by transaction.Date
                into filteredTransaction
                select new SplineChartType()
                {
                    Day = selectedTransaction.First().Date.ToString("dd/MM"), 
                    TotalIncome = selectedTransaction.Sum(s => s.Amount)
                };

            int[] range = new int[7];
            string[] daLast7Days = Enumerable.Range(0, 7).Select(i => startDate.AddDays(i).ToString("dd/MM")).ToArray();

            ViewBag.SplineChartData = from day in daLast7Days
                join income in incomeGr on day equals income.Day into joinedIncome
                from i in joinedIncome.DefaultIfEmpty()
                join expense in expenseGr on day equals expense.Day into joinedExpense
                from e in joinedExpense.DefaultIfEmpty()
                select new SplineChartType
                {
                    Day = day,
                    TotalIncome = i == null ? 0 : i.TotalIncome,
                    TotalExpense = e == null ? 0 : e.TotalExpense,
                };

            ViewBag.GridChart = selectedTransaction.OrderByDescending(o => o.Date).Take(7).ToList();
            
            ViewBag.SplineChart = selectedTransaction.ToList();
            return View();
        }

    }
}

class SplineChartType
{
    public SplineChartType()
    {
    }

    public string Day;
    public int TotalExpense;
    public int TotalIncome;
}
