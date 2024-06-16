using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    [Required(ErrorMessage = "Title is required")]
    public String Title { get; set; } = "";
    
    [Column(TypeName = "nvarchar(50)")]
    public String Icon { get; set; } = "";

    // Expense or income
    [Column(TypeName = "nvarchar(50)")]
    public String Type { get; set; } = "Expense";

    [NotMapped]
    public String? TitleWithIcon => this.Icon + " " + this.Title;
}