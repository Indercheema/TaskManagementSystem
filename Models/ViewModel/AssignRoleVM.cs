using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;

namespace TaskManagementSystem.Models.ViewModel
{
    public class AssignRoleVM
    {
        public List<SelectListItem> Roles { get; } = new List<SelectListItem>();

        public List<IdentityRole> Roles2 { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }

        public string? ViewMessage { get; set; }

        public void PopulateLists(HashSet<IdentityRole> role)
        {

            foreach (IdentityRole r in role)
            {
                Roles.Add(new SelectListItem(r.Name, r.Id));
            }
        }


        public AssignRoleVM() { }
        public AssignRoleVM(HashSet<IdentityRole> role)
        {
            PopulateLists(role);
        }
    }
}
