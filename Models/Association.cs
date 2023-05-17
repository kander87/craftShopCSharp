#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace CraftShop.Models;

// using statements and namespace removed for brevity
public class Association
{
    [Key]    
    public int AssociationId { get; set; } 
    public int UserId { get; set; }    
    public int CraftId { get; set; }

    [Required]
    [Range(1,Int32.MaxValue)]
    public int Quantity {get; set;}
    
    public User? User { get; set; }    
    public Craft? Craft { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;  
}
