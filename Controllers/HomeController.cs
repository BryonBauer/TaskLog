using System;
using System.Collections.Generic;
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
    #region Project
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
            newProject.ProjectCreator.UserId = loggedUser.UserId;
            dbContext.Add(newProject);
            dbContext.SaveChanges();
            return RedirectToAction("ViewProject", new { ProjectID = newProject.ProjectID });
        }     
        Console.WriteLine("Model State Invalid!");
            return View("NewProject"); // Model State does not persist through a redirect
    }

    // Read one Project
    [HttpGet("viewProject/project/{ProjectId}")]
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
    [HttpGet("delete/project/{ProjectId}")]
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

    // Read one Project
    public async Task<IActionResult> ReadProject(int ProjectId)
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

    [HttpPost("EditProject/project/{ProjectId}")]
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
#endregion

    #region Task
    // New Task form
    [HttpGet("newTask")]
    public IActionResult NewTask()
    {
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }
        ViewBag.UserId = HttpContext.Session.GetInt32("logged");
        return View("NewTask");
    }

    // Check if the new Task being submited is valid
    [HttpPost("processTask")]
    public IActionResult ProcessTask(TaskLog.Models.Task newTask)
    {
        Console.WriteLine("In ProcessTask!");
        if(ModelState.IsValid)
        {
            Console.WriteLine("Checking Model!");
            User loggedUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("logged"));
            newTask.TaskCreator = loggedUser;
            newTask.TaskCreator.UserId = loggedUser.UserId;
            dbContext.Add(newTask);
            dbContext.SaveChanges();
            return RedirectToAction("ViewTask", new { TaskID = newTask.TaskID });
        }     
        Console.WriteLine("Model State Invalid!");
            return View("NewTask"); // Model State does not persist through a redirect
    }

        // Read one Task
    [HttpGet("viewTask/task/{TaskId}")]
    public IActionResult ViewTask(int TaskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        TaskLog.Models.Task retrievedTask = dbContext.Tasks.FirstOrDefault(t => t.TaskID == TaskId);
        PopulateBag();
        return View("ViewTask", retrievedTask);
    }

    // Remove one Task
    [HttpGet("delete/task/{TaskId}")]
    public IActionResult DeleteTask(int TaskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        TaskLog.Models.Task retrievedTask = dbContext.Tasks.FirstOrDefault(t => t.TaskID == TaskId);
        dbContext.Remove(retrievedTask);
        dbContext.SaveChanges();
        PopulateBag();
        return RedirectToAction("Dashboard"); 
    }

    // Read one task
    public async Task<IActionResult> ReadTask(int taskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }

        var retrievedTask = await dbContext.Tasks
            .Include(t => t.TaskName)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TaskID == taskId);
        
        if(retrievedTask == null){
            return NotFound();
        }

        PopulateBag();
        return View(retrievedTask);
    }

    [HttpPost("EditTask/task/{taskId}")]
    public async Task<IActionResult> EditTask(int taskId)
    {
        System.Console.WriteLine("Updating task!");
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }

        var taskToUpdate = await dbContext.Tasks
            .FirstOrDefaultAsync(s => s.TaskID == taskId);

        Console.WriteLine($"task to update: {taskToUpdate}");

        if(await TryUpdateModelAsync<TaskLog.Models.Task>(
            taskToUpdate,
            "",
            t => t.TaskName, t => t.TaskDescription, t => t.DueDate, t => t.EstimatedTime)){
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
            return View(taskToUpdate);  
    }
    #endregion

    #region SubTask
        // New SubTask form
    [HttpGet("newSubTask")]
    public IActionResult NewSubTask()
    {
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }
        ViewBag.UserId = HttpContext.Session.GetInt32("logged");
        return View("NewSubTask");
    }

    // Check if the new SubTask being submited is valid
    [HttpPost("processSubTask")]
    public IActionResult ProcessSubTask(TaskLog.Models.SubTask newSubTask)
    {
        Console.WriteLine("In ProcessSubTask!");
        if(ModelState.IsValid)
        {
            Console.WriteLine("Checking Model!");
            User loggedUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("logged"));
            newSubTask.SubTaskCreator = loggedUser;
            newSubTask.SubTaskCreator.UserId = loggedUser.UserId;
            dbContext.Add(newSubTask);
            dbContext.SaveChanges();
            return RedirectToAction("ViewSubTask", new { SubTaskID = newSubTask.SubTaskID });
        }     
        Console.WriteLine("Model State Invalid!");
            return View("NewSubTask"); // Model State does not persist through a redirect
    }

        // Read one SubTask
    [HttpGet("viewSubTask/subtask/{subTaskId}")]
    public IActionResult ViewSubTask(int subTaskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        TaskLog.Models.SubTask retrievedSubTask = dbContext.SubTasks.FirstOrDefault(t => t.SubTaskID == subTaskId);
        PopulateBag();
        return View("ViewSubTask", retrievedSubTask);
    }

    // Remove one SubTask
    [HttpGet("delete/task/{subTaskId}")]
    public IActionResult DeleteSubTask(int subTaskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }
        TaskLog.Models.SubTask retrievedSubTask = dbContext.SubTasks.FirstOrDefault(t => t.SubTaskID == subTaskId);
        dbContext.Remove(retrievedSubTask);
        dbContext.SaveChanges();
        PopulateBag();
        return RedirectToAction("Dashboard"); 
    }

    // Read one task
    public async Task<IActionResult> ReadSubTask(int subTaskId)
    {
        int flag = CheckLogged();
        if(flag == 0)
        {
            return RedirectToAction("Login");
        }

        var retrievedSubTask = await dbContext.SubTasks
            .Include(t => t.SubTaskName)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.SubTaskID == subTaskId);
        
        if(retrievedSubTask == null){
            return NotFound();
        }

        PopulateBag();
        return View(retrievedSubTask);
    }

    [HttpPost("EditSubTask/task/{subTaskId}")]
    public async Task<IActionResult> EditSubTask(int subTaskId)
    {
        System.Console.WriteLine("Updating task!");
        int flag = CheckLogged();
        if (flag == 0) 
        {
            return RedirectToAction("Login");
        }

        var taskToUpdate = await dbContext.SubTasks
            .FirstOrDefaultAsync(s => s.SubTaskID == subTaskId);

        Console.WriteLine($"task to update: {taskToUpdate}");

        if(await TryUpdateModelAsync<TaskLog.Models.SubTask>(
            taskToUpdate,
            "",
            t => t.SubTaskName, t => t.SubTaskDescription, t => t.DueDate, t => t.EstimatedTime)){
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
            return View(taskToUpdate);  
    }
    #endregion
    
    public void PopulateBag() 
    {
        User loggedUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("logged"));
        List<Project> UsersCreatedProjects = dbContext.Projects
                .Include(p => p.ProjectCreator)
                .Where(u => u.ProjectCreatorUserId == loggedUser.UserId)   
                .ToList();

        List<TaskLog.Models.Task> UsersCreatedTasks = dbContext.Tasks
                .Include(t => t.TaskCreator)
                .Where(u => u.TaskCreatorUserId == loggedUser.UserId)   
                .ToList();

        List<SubTask> UsersCreatedSubTasks = dbContext.SubTasks
                .Include(t => t.SubTaskCreator)
                .Where(u => u.SubTaskCreatorUserId == loggedUser.UserId)   
                .ToList();
                
        ViewBag.Email = loggedUser.Email;
        ViewBag.UserId = loggedUser.UserId;
        ViewBag.LoggedUser = loggedUser;

        ViewBag.Projects = UsersCreatedProjects;
        ViewBag.Tasks = UsersCreatedTasks;
        ViewBag.SubTasks = UsersCreatedSubTasks;
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
