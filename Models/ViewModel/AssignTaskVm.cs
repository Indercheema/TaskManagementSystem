using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModel
{
    public class AssignTaskVm
    {
        public List<SelectListItem> Developers { get; } = new List<SelectListItem>();

        public Task? Task { get; set; }
        public int TaskId { get; set; }


        [Display(Name = "Assignees")]
        public string UserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }


        public AssignTaskVm(HashSet<ApplicationUser> developer)
        {
            foreach (ApplicationUser u in developer)
            {
                Developers.Add(new SelectListItem(u.FullName, u.Id.ToString()));
            }
        }

        public AssignTaskVm() { }
    }
}
