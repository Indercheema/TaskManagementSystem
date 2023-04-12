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
        [HttpGet]
        public IActionResult Index()
        {
            FilterByVM vm = new FilterByVM();
            HashSet<Project> projects = _context.Project
               .Include(p => p.Tasks)
               .OrderBy(p => p.Title)
               .ToHashSet();

            vm.Projects= projects;

            return View(vm);

        }


        [HttpPost]
        public IActionResult Index(FilterByVM vm)
        {
            if(vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Projects = _context.Project.Include(p => p.Tasks.OrderBy(t => t.RequiredHours)).OrderBy(p => p.Title).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Projects = _context.Project.Include(p => p.Tasks.OrderByDescending(t => t.RequiredHours)).OrderBy(p => p.Title).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Projects = _context.Project.Include(p => p.Tasks.OrderBy(t => t.Priority)).OrderBy(p => p.Title).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Projects = _context.Project.Include(p => p.Tasks.OrderByDescending(t => t.Priority)).OrderBy(p => p.Title).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.CompletedTask))
            {
                vm.Projects = _context.Project.Include(p => p.Tasks.Where(t => t.IsCompleted == false)).OrderBy(p => p.Title).ToHashSet();
            }

            return View(vm);
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


        /*
        public IActionResult GetAllDevelopersInProject(int projectId)
        {
            Project selectedProject = _context.Project.Find(projectId);

            if (selectedProject != null)
            {
                return NotFound();
            }

            HashSet<string> DevelopersInProjectId = _context.ProjectContributor.Where(pc => pc.ProjectId == selectedProject.Id).Select(pc => pc.UserId).ToHashSet();

            HashSet<ApplicationUser> DevelopersInProject = new HashSet<ApplicationUser>();

            foreach(string s in DevelopersInProjectId)
            {
                ApplicationUser developer = _context.Users.Find(s);

                DevelopersInProject.Add(developer);
            }

            return View(DevelopersInProject);
        }
        */

        [HttpGet]

        public async Task<IActionResult> CreateTask(int projectid)
        {
            Project? selectedProject = _context.Project.Find(projectid);

            if (selectedProject == null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = selectedProject.Id;

            return View();
        }

        [HttpPost]

        public IActionResult CreateTask([Bind("Title","RequiredHours","Priority","ProjectId")]Task task)
        {
            try
            {
                Task newTask = new Task();
                newTask.Title = task.Title;
                newTask.RequiredHours = task.RequiredHours;
                newTask.IsCompleted = false;
                newTask.Priority = task.Priority;
                newTask.Project = task.Project;
                newTask.ProjectId = task.ProjectId;

                
                if (TryValidateModel(newTask))
                {
                    _context.Add(newTask);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(task);
                }

            }
            catch(Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        public async Task<IActionResult> AssignTask(int taskId)
        {
            Task? selectedTask = _context.Task.Find(taskId);

            if (selectedTask == null)
            {
                return NotFound();
            }

            ViewBag.TaskId = selectedTask.Id;



            HashSet<string> DevelopersInProjectId = _context.ProjectContributor.Where(pc => pc.ProjectId == selectedTask.ProjectId).Select(pc => pc.UserId).ToHashSet();

            HashSet<ApplicationUser> DevelopersInProject = new HashSet<ApplicationUser>();

            foreach (string s in DevelopersInProjectId)
            {
                ApplicationUser developer = _context.Users.Find(s);

                DevelopersInProject.Add(developer);
            }


            AssignTaskVm vm = new AssignTaskVm(DevelopersInProject);

            return View(vm);
        }

        [HttpPost]
        public IActionResult AssignTask(AssignTaskVm vm)
        {

            try
            {
                Task selectedTask = _context.Task.Find(vm.TaskId);

                if (selectedTask != null)
                {

                    List<string> TaskDevelopersId = Request.Form["UserId"].ToList();

                    List<ApplicationUser> Taskdevelopers = new List<ApplicationUser>();

                    foreach (string pd in TaskDevelopersId)
                    {
                        ApplicationUser user = _context.Users.Find(pd);

                        if (user != null)
                        {
                            Taskdevelopers.Add(user);
                        }
                    }

                    foreach (ApplicationUser u in Taskdevelopers)
                    {
                        TaskContributor newTaskContributor = new TaskContributor();

                        newTaskContributor.Task = selectedTask;
                        newTaskContributor.ApplicationUser = u;
                        newTaskContributor.UserId = u.Id;
                        newTaskContributor.TaskId = selectedTask.Id;

                        u.TaskContributors.Add(newTaskContributor);
                        selectedTask.TaskContributors.Add(newTaskContributor);
                        _context.TaskContributor.Add(newTaskContributor);
                    }

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest();
                }
                
                
            } 
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
