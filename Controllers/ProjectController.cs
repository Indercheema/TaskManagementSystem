using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModel;
using Project = TaskManagementSystem.Models.Project;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="Project Manager")]
    public class ProjectController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectController(TaskContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public IActionResult Index()
        {
            HashSet<Project> projects = _context.Project
                .Include(p => p.Tasks)
                .ToHashSet();
            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            HashSet<ApplicationUser> DevelopersInRole = new HashSet<ApplicationUser>();

            IdentityRole role = _roleManager.Roles.Where(r => r.Name == "Developer").FirstOrDefault();

            HashSet<string> DevelopersInRoleId = _context.UserRoles.Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToHashSet();

            foreach(string user in DevelopersInRoleId)
            {
                ApplicationUser developer = _context.Users.Find(user);
                DevelopersInRole.Add(developer);
            }

            CreateProjectVm vm = new CreateProjectVm(DevelopersInRole);

            return View(vm);
        }

        [HttpPost] 
        public IActionResult Create([Bind("Title","UserId")]CreateProjectVm vm)
        {
            try
            {

                List<string> ProjectDevelopersId = Request.Form["UserId"].ToList();

                List<ApplicationUser> Projectdevelopers = new List<ApplicationUser>();

                foreach (string pd in ProjectDevelopersId)
                {
                    ApplicationUser user = _context.Users.Find(pd);

                    if (user != null)
                    {
                        Projectdevelopers.Add(user);
                    }
                }

                Project newProject = new Project();
                newProject.Title = vm.Title;

                _context.Project.Add(newProject);
                _context.SaveChanges();

                foreach (ApplicationUser u in Projectdevelopers)
                {
                    ProjectContributor newProjectContributor = new ProjectContributor();

                    newProjectContributor.Project = newProject;
                    newProjectContributor.ApplicationUser = u;
                    newProjectContributor.UserId = u.Id;
                    newProjectContributor.ProjectId = newProject.Id;

                    u.ProjectContributors.Add(newProjectContributor);
                    newProject.ProjectContributors.Add(newProjectContributor);
                }

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public IActionResult CreateTask(int projectId)
        {
            Project selectedProject = _context.Project.Find(projectId);
            
            if (selectedProject != null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = selectedProject.Id;
            return View();
        }

        public IActionResult CreateTask([Bind("Title","RequiredHours","IsComleted","Priority","ProjectId")]Task task)
        {

            try
            {
                Project selectedProject = _context.Project.Find(task.ProjectId);
                Task newTask = new Task();

                if (ModelState.IsValid)
                {
                    newTask.Id = task.Id;
                    newTask.Title = task.Title;
                    newTask.IsCompleted = false;
                    newTask.RequiredHours = task.RequiredHours;
                    newTask.Priority = task.Priority;
                    newTask.Project= task.Project;

                    selectedProject.Tasks.Add(newTask);
                }

                
                _context.SaveChanges();
                return RedirectToAction("Index");
            } 
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
