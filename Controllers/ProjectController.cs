using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Drawing.Drawing2D;
using PagedList;
using PagedList.Mvc;
using System.Drawing.Printing;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.ViewModel;
using Project = TaskManagementSystem.Models.Project;
using Task = TaskManagementSystem.Models.Task;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Project Manager")]
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
        public IActionResult Index(int page = 1)
        {
            FilterByVM vm = new FilterByVM();
            HashSet<Project> allProjects = _context.Project
               .Include(p => p.Tasks)
               .OrderBy(p => p.Title)
            .ToHashSet();
            ViewBag.TotalPage = Math.Ceiling(allProjects.Count() / 1.0);
            HashSet<Task> AllTasksInProjects = new HashSet<Task>();
            foreach (Task t in allProjects.SelectMany(p => p.Tasks))
            {
                AllTasksInProjects.Add(t);
            }

            //ViewBag.tasks = Math.Ceiling(projects.SelectMany(p => p.Tasks).Count() / 2.0);

            ViewBag.tasks = Math.Ceiling(AllTasksInProjects.Count() / 3.0);

            //vm.Tasks = projects.FirstOrDefault().Tasks.Skip((page - 1) * 2).Take(2).ToHashSet();
            //vm.Tasks = projects.SelectMany(p => p.Tasks.Skip((page - 1) * 2).Take(2)).ToHashSet();
            vm.Tasks = AllTasksInProjects.Skip((page - 1) * 3).Take(3).ToHashSet();
            if (page == 1)
            {
                vm.Projects = allProjects.Where(p => p.Tasks.Count == 0).ToHashSet();
            }
            return View(vm);

        }


        [HttpPost]
        public IActionResult Index(int page, FilterByVM vm)
        {
            HashSet<Project> allProjects = _context.Project
              .Include(p => p.Tasks)
              .OrderBy(p => p.Title)
           .ToHashSet();
            ViewBag.TotalPage = Math.Ceiling(allProjects.Count() / 1.0);
            HashSet<Task> AllTasksInProjects = new HashSet<Task>();
            foreach (Task t in allProjects.SelectMany(p => p.Tasks))
            {
                AllTasksInProjects.Add(t);
            }
            ViewBag.tasks = Math.Ceiling(AllTasksInProjects.Count() / 3.0);

            //vm.Tasks = tasks.Skip((page - 1) * 3).Take(3).ToHashSet();
            if (page == 1)
            {
                vm.Projects = allProjects.Where(p => p.Tasks.Count == 0).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Tasks = AllTasksInProjects.Skip((page - 1) * 3).Take(3).OrderBy(t => t.RequiredHours).ToHashSet();
            }


            if (vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Tasks = AllTasksInProjects.Skip((page - 1) * 3).Take(3).OrderByDescending(t => t.RequiredHours).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Tasks = AllTasksInProjects.Skip((page - 1) * 3).Take(3).OrderBy(t => t.Priority).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Tasks = AllTasksInProjects.Skip((page - 1) * 3).Take(3).OrderByDescending(t => t.Priority).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.CompletedTask))
            {
                vm.Tasks = AllTasksInProjects.Where(t => t.IsCompleted == false).Skip((page - 1) * 3).Take(3).ToHashSet();
            }

            if (vm.Filter.Equals(FilterBy.AssignedTask))
            {

                HashSet<Task> TasksWithoutDevelopers = new HashSet<Task>();

                HashSet<Project> projects = _context.Project.ToHashSet();

                HashSet<Project> projectWithoutAssignedTask = new HashSet<Project>();

                HashSet<Task> allTasks = _context.Task.ToHashSet();

                HashSet<TaskContributor> taskContributors = _context.TaskContributor.ToHashSet();

                foreach (Project p in projects)
                {
                    foreach (Task t in p.Tasks)
                    {
                        bool hasUnAssignedTask = false;

                        foreach (TaskContributor tc in taskContributors)
                        {

                            if (t.Id == tc.TaskId)
                            {
                                hasUnAssignedTask = true;
                            }

                        }
                        if (!hasUnAssignedTask)
                        {
                            TasksWithoutDevelopers.Add(t);
                            projectWithoutAssignedTask.Add(p);

                        }

                    }


                }
                vm.Tasks = TasksWithoutDevelopers.Skip((page - 1) * 3).Take(3).OrderBy(t => t.Project.Title).ToHashSet();
            }

            return View(vm);
        }

        [HttpGet]

        public IActionResult Details(int projectid)
        {
            FilterByVM vm = new FilterByVM();
            Project project = _context.Project.Include(p => p.Tasks).FirstOrDefault(p => p.Id == projectid);
            vm.ProjectId = projectid;
            vm.Project = project;
            return View(vm);
        }

        [HttpPost]

        public IActionResult Details(FilterByVM vm)
        {

            if (vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Project = _context.Project.Include(p => p.Tasks.OrderBy(t => t.RequiredHours)).First(p => p.Id == vm.ProjectId);
            }

            if (vm.Filter.Equals(FilterBy.Hours) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Project = _context.Project.Include(p => p.Tasks.OrderByDescending(t => t.RequiredHours)).First(p => p.Id == vm.ProjectId);
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Ascending))
            {
                vm.Project = _context.Project.Include(p => p.Tasks.OrderBy(t => t.Priority)).First(p => p.Id == vm.ProjectId);
            }

            if (vm.Filter.Equals(FilterBy.Priority) && vm.Order.Equals(OrderBy.Descending))
            {
                vm.Project = _context.Project.Include(p => p.Tasks.OrderByDescending(t => t.Priority)).First(p => p.Id == vm.ProjectId);
            }

            if (vm.Filter.Equals(FilterBy.CompletedTask))
            {
                vm.Project = _context.Project.Include(p => p.Tasks.Where(t => t.IsCompleted == false)).First(p => p.Id == vm.ProjectId);
            }

            if (vm.Filter.Equals(FilterBy.AssignedTask))
            {

                vm.Project = _context.Project.First(p => p.Id == vm.ProjectId);

                HashSet<Task> tasks = new HashSet<Task>();

                HashSet<Project> projects = _context.Project.ToHashSet();

                HashSet<Project> project2 = new HashSet<Project>();

                HashSet<Task> allTasks = _context.Task.ToHashSet();

                HashSet<TaskContributor> taskContributors = _context.TaskContributor.ToHashSet();

                //foreach (Project p in projects)
                //{
                foreach (Task t in vm.Project.Tasks)
                {
                    bool hasUnAssignedTask = false;

                    foreach (TaskContributor tc in taskContributors)
                    {

                        if (t.Id == tc.TaskId)
                        {
                            hasUnAssignedTask = true;
                        }

                    }
                    if (!hasUnAssignedTask)
                    {
                        tasks.Add(t);

                    }

                }


                //}
                vm.Tasks = tasks;
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
        public IActionResult Create([Bind("Title", "UserId")] CreateProjectVm vm)
        {
            try
            {
                ApplicationUser manager = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

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
                newProject.Manager = manager;
                newProject.MangerId = manager.Id;
                newProject.Title = vm.Title;

                if (ModelState.IsValid)
                {
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
                }


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

        public IActionResult CreateTask([Bind("Title", "RequiredHours", "Priority", "ProjectId")] Task task)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
