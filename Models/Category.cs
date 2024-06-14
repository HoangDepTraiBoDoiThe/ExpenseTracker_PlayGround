using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public int Title { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    public int Icon { get; set; }

    // Expense or income
    [Column(TypeName = "nvarchar(50)")]
    public int Type { get; set; }

}