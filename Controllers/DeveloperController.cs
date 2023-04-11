using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
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
            HashSet<ProjectContributor> projects = _context.ProjectContributor.Include(pc => pc.Project).ThenInclude(p => p.Tasks).Where(pc => pc.UserId == user.Id).ToHashSet();
             
            return View(projects);
        }
    }
}
