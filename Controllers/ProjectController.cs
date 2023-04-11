using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModel;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(TaskContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

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
            HashSet<ApplicationUser> users = _userManager.Users.Where(u => u.UserName != "admin34@gmail.com").ToHashSet();

            CreateProjectVm vm = new CreateProjectVm(users);

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
    }
}
