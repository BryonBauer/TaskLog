using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TaskLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TaskLog.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        #region Register and Login
        [HttpPost("register")]
            public IActionResult Register(User newUser) 
            {
                if (ModelState.IsValid) {
                    if (dbContext.Users.Any (u => u.Email == newUser.Email)) {
                        ModelState.AddModelError ("Email",
                            "Email already in use. Please log in.");
                        return View("Index");
                    }
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword (newUser, newUser.Password);
                    dbContext.Users.Add (newUser);
                    dbContext.SaveChanges ();
                    User loggedUser = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                    HttpContext.Session.SetInt32("logged", loggedUser.UserId);
                    return RedirectToAction("Dashboard");
                } else {
                    return View ("Index"); 
                }
            }

            [HttpGet("login")]
            public IActionResult Login()
            {
                return View();
            }

            [HttpPost("processlogin")]
            public IActionResult ProcessLogin(LoginUser userSubmission)
            {
                Console.WriteLine("Processing Login!");
                if(ModelState.IsValid)
                {
                    Console.WriteLine("Checking Model!");
                    // If inital ModelState is valid, query for a user with provided email
                    var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                    // If no user exists with provided email
                    if(userInDb == null)
                    {
                        // Add an error to ModelState and return to View!
                        ModelState.AddModelError("Email", "Invalid Email/Password");
                        return View("Login");
                    }

                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                    
                        // result can be compared to 0 for failure
                        if(result == 0)
                        {
                            Console.WriteLine("It failed somehow!");
                            ModelState.AddModelError("Email", "Invalid Email/Password");
                            return View("Login");
                        }
                    User loggedUser = userInDb;
                    HttpContext.Session.SetInt32("logged", loggedUser.UserId);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return View("Login");
                }
            }
        #endregion

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        Console.WriteLine("Getting Dashboard");
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }
        PopulateBag();
        return View();
    }

    // New Project form
    [HttpGet("newProject")]
    public IActionResult NewProject()
    {
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }
        ViewBag.UserId = HttpContext.Session.GetInt32("logged");
        return View("NewProject");
    }

    // Check if the new Project being submited is valid
    [HttpPost("processProject")]
    public IActionResult ProcessProject(Project newProject)
    {
        Console.WriteLine("In ProcessProject!");
        if(ModelState.IsValid)
        {
            Console.WriteLine("Checking Model!");
            User loggedUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("logged"));
            newProject.ProjectCreator = loggedUser;
            newProject.UserId = loggedUser.UserId;
            dbContext.Add(newProject);
            dbContext.SaveChanges();
            return RedirectToAction("ViewProject", new { ProjectID = newProject.ProjectID });
        }     
        Console.WriteLine("Model State Invalid!");
            return View("NewProject"); // Model State does not persist through a redirect
    }

    // Read one Project
    [HttpGet("viewProject/{ProjectId}")]
    public IActionResult ViewProject(int ProjectId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        Project retrievedProject = dbContext.Projects.FirstOrDefault(t => t.ProjectID == ProjectId);
        PopulateBag();
        return View("ViewProject", retrievedProject);
    }

    // Remove one Project
    [HttpGet("delete/{ProjectId}")]
    public IActionResult DeleteProject(int ProjectId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        Project retrievedProject = dbContext.Projects.FirstOrDefault(t => t.ProjectID == ProjectId);
        dbContext.Remove(retrievedProject);
        dbContext.SaveChanges();
        PopulateBag();
        return RedirectToAction("Dashboard"); 
    }

    // Edit one Project
    public async Task<IActionResult> Edit(int ProjectId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }

        var retrievedProject = await dbContext.Projects
            .Include(p => p.ProjectName)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProjectID == ProjectId);
        
        if(retrievedProject == null){
            return NotFound();
        }

        PopulateBag();
        return View(retrievedProject);
    }

    [HttpPost("EditProject/{ProjectId}")]
    public async Task<IActionResult> EditProject(int ProjectId)
    {
        System.Console.WriteLine("Updating Project!");
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }

        var projectToUpdate = await dbContext.Projects
            .FirstOrDefaultAsync(s => s.ProjectID == ProjectId);

        Console.WriteLine($"Project to update: {projectToUpdate}");

        if(await TryUpdateModelAsync<Project>(
            projectToUpdate,
            "",
            p => p.ProjectName, p => p.ProjectDescription, p => p.DueDate, p => p.EstimatedTime)){
            try{
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return RedirectToAction(nameof(Dashboard));
        }
            return View(projectToUpdate);  
    }

    public void PopulateBag() 
    {
        User loggedUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("logged"));
        List<Project> ProjectWithUsers = dbContext.Projects
                .Include (g => g.ProjectCreator)
                .ToList ();
        ViewBag.UsersProjects = ProjectWithUsers;
        ViewBag.Email = loggedUser.Email;
        ViewBag.UserId = loggedUser.UserId;
        ViewBag.Projects = ProjectWithUsers;  
        ViewBag.LoggedUser = loggedUser;
    }

    public int CheckLogged ()
    {
        int flag = 1;  
        if (HttpContext.Session.GetInt32 ("logged") == null) {
            flag = 0;
            TempData["alertMessage"] = "<p style='color:red;'>Please login or register.</p>";
        }
        return flag;
    }

        [HttpGet("logout")]
        public IActionResult Logout (){
            HttpContext.Session.Clear();   
            return View("Login");
        }
    }
}
