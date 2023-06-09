#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace CraftShop.Models;

public class MyViewModel
    {
        public User? User  { get; set; }
        public List<User> AllUsers  { get; set; }
        // public List<User> newProd  { get; set; }

        public Craft? Craft  { get; set; }
        public List<Craft> AllCrafts  { get; set; }
        // public List<Category> newCat  { get; set; }

        public Association? Association  { get; set; }
        public List<Association> Associations { get; set; } = new List<Association>();

        
    }