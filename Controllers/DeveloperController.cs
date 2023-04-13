using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public DeveloperController(TaskContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            ApplicationUser user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            HashSet<ProjectContributor> projects = _context.ProjectContributor
                .Include(pc => pc.Project)
                .ThenInclude(p => p.Tasks.Where(t => t.TaskContributors.Any(tc => tc.UserId == user.Id)))
                .ThenInclude(t => t.TaskContributors)
                .Where(pc => pc.UserId == user.Id)
                .ToHashSet();

            return View(projects);
        }

        [HttpGet]

        public IActionResult UpdateRequiredHours(int? id)
        {
            ViewBag.taskid = id;
            return View();
        }
        [HttpPost]

        public IActionResult UpdateRequiredHours(int id, Task task)
        {
            Task selectedTask = _context.Task.Find(id);

            if (selectedTask == null && task != null)
            {
                return BadRequest();
            }
            else if (task != null)
            {
                selectedTask.RequiredHours = task.RequiredHours;
                _context.Task.Update(selectedTask);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");

        }
        

        public IActionResult MarkAsCompleted(int? id, Task task)
        {
            Task selectedTask = _context.Task.Find(id);

            if (selectedTask == null && task != null)
            {
                return BadRequest();
            }
            else if (task != null)
            {
                selectedTask.IsCompleted = true;
                _context.Task.Update(selectedTask);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



    }
}
