#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftShop.Models;

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(2, ErrorMessage ="Username must be at least 2 characters.")]
        [UniqueUsername]
        public string Username { get; set; } 

        [Required (ErrorMessage = "Email is required")]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; } 

        [Required (ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;        

        [NotMapped]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }  

        public List<Craft> AllCrafts { get; set; } = new List<Craft>();
        public List<Association> Associations { get; set; } = new List<Association>();
    }


public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Email is required!");
        }
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if(_context.Users.Any(e => e.Email == value.ToString()))
        {
            return new ValidationResult("Email must be unique!");
        } else {
            return ValidationResult.Success;
        }
    }
}


public class UniqueUsernameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Username is required!");
        }
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if(_context.Users.Any(e => e.Username == value.ToString()))
        {
            return new ValidationResult("Username is already taken!");
        } else {
            return ValidationResult.Success;
        }
    }
}
