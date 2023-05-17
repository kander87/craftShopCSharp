#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;


namespace CraftShop.Models;

    public class Craft
    {
        [Key]
        public int CraftId { get; set; }

        [Required(ErrorMessage = "Image link is required")]
        [DataType(DataType.ImageUrl)]
        [URL]
        public string Image { get; set; } 

        [Required(ErrorMessage = "Title of the item is required")]
        [MinLength(2, ErrorMessage ="Title must be at least 2 characters.")]
        public string Title { get; set; } 

        [Required (ErrorMessage = "Price is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be at least $0.01") ]
        public double? Price { get; set; }

        [Required (ErrorMessage = "Quanity is required")]
        [Range(0, Int32.MaxValue, ErrorMessage ="Quantity must be at least 1.")]
        public int? Quantity { get; set; } 

        [Required(ErrorMessage = "Description of the item is required")]
        [MinLength(2, ErrorMessage ="Description must be at least 2 characters.")]
        public string Description { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;        

        public List<Association> Associations { get; set; } = new List<Association>();

        public int UserId  { get; set; }
        public User? Creator  { get; set; }
    }

public class URLAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? ImageUrl = value as string;

        if(ImageUrl == null || !ImageUrl.EndsWith(".png",StringComparison.OrdinalIgnoreCase) && !ImageUrl.EndsWith(".jpg",StringComparison.OrdinalIgnoreCase) )
        {
            return new ValidationResult("Invalid URL! Image must be a jpg or png URL!");
        } else {
            return ValidationResult.Success;
        }
    }
}

