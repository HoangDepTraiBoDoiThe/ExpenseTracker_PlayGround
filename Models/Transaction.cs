using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public class Transaction
{
    [Key]
    public int TransactionId { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int Amount { get; set; }
    [Column(TypeName = "nvarchar(75)")]
    public string? Note { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    [NotMapped]
    public string? CategoryWithIcon => Category == null ? "" : Category.TitleWithIcon;
    
    [NotMapped]
    public string? FormatedAmount => ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("C0", new System.Globalization.CultureInfo("es-US"));
}