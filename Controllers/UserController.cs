using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CraftShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CraftShop.Controllers;    
public class UserController : Controller
{    
    private readonly ILogger<UserController> _logger;
    private MyContext _context;         
    public UserController(ILogger<UserController> logger, MyContext context)    
    {        
        _logger = logger;
        _context = context;    
    }   

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpPost("user/create")]   
    public IActionResult Create(User newUser)    
    {        
        if(ModelState.IsValid)        
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();   
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);            
            _context.Add(newUser);
            _context.SaveChanges();  
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("Username", newUser.Username);

            return RedirectToAction("Dashboard");
        } else {
            return View("Index");
        }   
    }

[HttpPost("user/login")]
public IActionResult Login(LogUser userSubmission)
{    
    if(ModelState.IsValid)    
    {        
        User? userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LEmail);        
        if(userInDb == null)        
            {   
            ModelState.AddModelError("Email", "Invalid Email/Password");            
            return View("Index");        
            }   
        PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();                    
        var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LPassword);      
        if(result == 0)        
        {   
            ModelState.AddModelError("LEmail", "Invalid Email/Password");
            return View("Index");
        } 
        else 
        {
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            HttpContext.Session.SetString("Username", userInDb.Username);
            return RedirectToAction("Dashboard");
        } 
    }
    else 
    {
        return View("Index");
}}

    [HttpGet("/users/dashboard")]
    public IActionResult Dashboard()
    {
        MyViewModel MyModel = new MyViewModel()
        {
            User = _context.Users
            .Include(assoc => assoc.Associations)
            .SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId")),
            
            Associations = _context.Associations
            .Include(craft=>craft.Craft)
            .ThenInclude(craft=> craft.Creator)
            .Where(assoc=> assoc.Craft.Creator.UserId == HttpContext.Session.GetInt32("UserId"))
            .ToList()
        };
        return View("Dashboard", MyModel);
    }

    [HttpGet("/users/orderhistory")]
    public IActionResult OrderHistory()
    {
        MyViewModel MyModel = new MyViewModel()
        {
            User = _context.Users
            .Include(assoc => assoc.Associations)
            .ThenInclude(craft=> craft.Craft)
            .ThenInclude(craft=> craft.Creator)
            // .Where(user => user.UserId == HttpContext.Session.GetInt32("UserId"))
            .SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId")),
            
            Associations = _context.Associations
            .Include(craft=>craft.Craft)
            .ThenInclude(craft=> craft.Creator)
            .Include(user=> user.User)
            .Where(assoc=> assoc.Craft.Creator.UserId == HttpContext.Session.GetInt32("UserId"))
            .ToList()
        };
        return View("OrderHistory", MyModel);
    }


public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if(userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}