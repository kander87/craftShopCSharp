using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CraftShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CraftShop.Controllers;
public class CraftController : Controller
{
    private readonly ILogger<CraftController> _logger;
    private MyContext _context;
    public CraftController(ILogger<CraftController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [SessionCheck]
    [HttpGet("/allcrafts")]
    public IActionResult AllCrafts()
    {
        MyViewModel MyModel = new MyViewModel()
        {
            AllCrafts = _context.Crafts
            .OrderBy(craft => craft.Quantity)
            .Include(craft => craft.Associations)
            .Include(creator => creator.Creator)
            .ToList()
        };
        return View("AllCrafts", MyModel);
    }

    [SessionCheck]
    [HttpGet("/craft/new")]
    public IActionResult NewCraft()
    {
        return View("NewCraft");
    }

    [SessionCheck]
    [HttpPost("/craft/create")]
    public IActionResult CraftCreate(Craft newCraft)
    {
        if (ModelState.IsValid)
        {
            newCraft.UserId = (int)HttpContext.Session.GetInt32("UserId");
            _context.Add(newCraft);
            _context.SaveChanges();
            return RedirectToAction("AllCrafts");
        }
        else
        {
            return View("NewCraft");
        }
    }

    [SessionCheck]
    [HttpPost("/{craftId}/delete")]
    public IActionResult CraftDelete(int craftId)
    {
        Console.WriteLine(craftId);
        Craft? CrafttoDelete = _context.Crafts.SingleOrDefault(craft => craft.CraftId == craftId);
        Console.WriteLine(CrafttoDelete);
        if (CrafttoDelete.UserId != HttpContext.Session.GetInt32("UserId"))
        {
            return RedirectToAction("AllCrafts");
        }
        _context.Crafts.Remove(CrafttoDelete);
        _context.SaveChanges();
        return RedirectToAction("AllCrafts");
    }

    [HttpGet("/edit/{craftId}")]
    public IActionResult CraftEdit(int craftId)
    {
        Craft? oneCraft = _context.Crafts.SingleOrDefault(craft => craft.CraftId == craftId);
        if (oneCraft == null)
        {
            return RedirectToAction("AllCrafts");
        }
        return View("EditCraft", oneCraft);
    }

    [HttpGet("/CraftUpdate/{craftId}")]
    public IActionResult CraftUpdate(Craft newCraft, int craftId)
    {
        Craft? OldCraft = _context.Crafts.SingleOrDefault(craft => craft.CraftId == craftId);
        if (OldCraft == null)
        {
            return RedirectToAction("AllCrafts");
        }
        if (ModelState.IsValid)
        {
            OldCraft.Image = newCraft.Image;
            OldCraft.Title = newCraft.Title;
            OldCraft.Price = newCraft.Price;
            OldCraft.Quantity = newCraft.Quantity;
            OldCraft.Description = newCraft.Description;
            OldCraft.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("AllCrafts");
        }
        else
        {
            return View("CraftEdit", OldCraft);
        }
    }

    [SessionCheck]
    [HttpPost("/{craftId}/buy")]
    public IActionResult Buy(Association order, int craftId)
    {
        Console.WriteLine("******************************************************************");
        Console.WriteLine(craftId);
        int UserId = (int)HttpContext.Session.GetInt32("UserId");
        order.UserId = UserId;
        if (ModelState.IsValid)
        {
            Craft? craft = _context.Crafts.FirstOrDefault(c => c.CraftId == craftId);
            if (craft is not null)
            {
                craft.Quantity -= order.Quantity;
                _context.Add(order);
                _context.SaveChanges();
                return RedirectToAction("ViewCraft", new { craftId = craft.CraftId });
            }
        }
        return RedirectToAction("ViewCraft", order.CraftId);
    }


    [SessionCheck]
    [HttpGet("/craft/{craftId}")]
    public IActionResult ViewCraft(int craftId)
    {
        ViewBag.craft = _context.Crafts
        .Include(craft => craft.Associations)
        .Include(craft => craft.Creator)
        .SingleOrDefault(craft => craft.CraftId == craftId);

        MyViewModel MyModel = new MyViewModel(){};
        return View("ViewCraft", MyModel);
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}


    // [SessionCheck]
    // [HttpGet("/{craftId}/Buy")]
    // public IActionResult Buy(Craft updateCraft, int craftId, int QuantityToBuy){
    //     Console.WriteLine("******************************************************************");
    //     Console.WriteLine(QuantityToBuy);
    //     Association newPurchase= new Association(){
    //         CraftId = craftId,
    //         UserId = (int)HttpContext.Session.GetInt32("UserId"),
    //     };
    //     _context.Associations.Add(newPurchase);
    //     _context.SaveChanges();

    //     Craft? OldCraft = _context.Crafts.SingleOrDefault(craft => craft.CraftId ==craftId);
    //     if (OldCraft == null){
    //         return RedirectToAction("AllCrafts");
    //     }
    //     if(QuantityToBuy > 0 && QuantityToBuy <= OldCraft.Quantity)
    //     {   
    //         OldCraft.Quantity = OldCraft.Quantity -QuantityToBuy;
    //         OldCraft.UpdatedAt = DateTime.Now;
    //         _context.SaveChanges();
    //         return RedirectToAction("AllCrafts");
    //         } else {
    //         ModelState.AddModelError("Quantity", "Invalid purchase amount!");
    //         return RedirectToAction("ViewCraft", OldCraft);
    //     }
    // }

    // [SessionCheck]
    // [HttpGet("/{craftId}/{associationId}/RSVPRemove")]
    // public IActionResult RSVPRemove(int? craftId, int associationId)
    // {
    //     Association? AssociationToDelete = _context.Associations.SingleOrDefault(i => i.AssociationId == associationId);
    //     craftId = AssociationToDelete.CraftId;
    //     _context.Associations.Remove(AssociationToDelete);
    //     _context.SaveChanges();
    //     return RedirectToAction("AllCrafts");
    // }