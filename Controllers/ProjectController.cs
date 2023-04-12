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
        public async Task<IActionResult> Create()
        {
            IdentityRole role = _roleManager.Roles.Where(r => r.Name == "Developer").FirstOrDefault();


            HashSet<string> DevelopersInRoleId = _context.UserRoles.Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToHashSet();

            HashSet<ApplicationUser> DevelopersInRole = new HashSet<ApplicationUser>();

            foreach (string user in DevelopersInRoleId)
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


        public HashSet<ApplicationUser> GetAllDevelopersInProject(Task task)
        {

            HashSet<string> AssignedProjectDevelopersId = _context.ProjectContributor.Where(pc => pc.ProjectId == task.ProjectId).Select(pc => pc.UserId).ToHashSet();

            HashSet<ApplicationUser> AssignedProjectDevelopers = new HashSet<ApplicationUser>();

            foreach (string s in AssignedProjectDevelopersId)
            {
                ApplicationUser developer = _context.Users.Find(s);

                AssignedProjectDevelopers.Add(developer);
            }

            return AssignedProjectDevelopers;
        }
        

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

            AssignTaskVm vm = new AssignTaskVm(GetAllDevelopersInProject(selectedTask));

            return View(vm);
        }

        [HttpPost]
        public IActionResult AssignTask(AssignTaskVm vm)
        {

            try
            {
                Task? selectedTask = _context.Task.Find(vm.TaskId);

                if (selectedTask != null)
                {

                    List<string> AssignedTaskDevelopersId = Request.Form["UserId"].ToList();

                    List<ApplicationUser> AssignedTaskDevelopers = new List<ApplicationUser>();

                    foreach (string developer in AssignedTaskDevelopersId)
                    {
                        ApplicationUser? user = _context.Users.Find(developer);

                        if (user != null)
                        {
                            AssignedTaskDevelopers.Add(user);
                        }
                    }

                    foreach (ApplicationUser u in AssignedTaskDevelopers)
                    {
                        if (_context.TaskContributor.Any(tc => tc.UserId == u.Id && tc.TaskId == selectedTask.Id))
                        {
                            ViewBag.Message = $"{u.FullName} already assigned to this task";

                            ViewBag.TaskId = selectedTask.Id;
                            AssignTaskVm newVm = new AssignTaskVm(GetAllDevelopersInProject(selectedTask));
                            return View(newVm);
                        }
                        else
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
