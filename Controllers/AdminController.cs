using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models.ViewModel;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(TaskContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> users = _userManager.Users.Where(u => u.UserName != "admin34@gmail.com").ToList();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            AssignRoleVM vm = new AssignRoleVM(_roleManager.Roles.Where(role => role.Name != "Administrator").ToHashSet());
            ApplicationUser user = _context.Users.Find(id);
            vm.UserId = user.Id;

            if (user != null)
            {
                return View(vm);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleVM vm)
        {
            ApplicationUser foundUser = _context.Users.Find(vm.UserId);

            IdentityRole role = _context.Roles.Find(vm.RoleId);

            if (!_context.UserRoles.Any(ul => ul.UserId == vm.UserId && ul.RoleId == vm.RoleId))
            {
                await _userManager.AddToRoleAsync(foundUser, role.Name);
                _context.SaveChanges();
                vm.ViewMessage = $"{role.Name} Role Succesfully added to {foundUser.FullName}";
            } else
            {
                vm.ViewMessage = $"'{foundUser.FullName}' already has '{role.Name}' Role";
            }

            vm.PopulateLists(_roleManager.Roles.ToHashSet());

            return View(vm);



        }


    }
}

