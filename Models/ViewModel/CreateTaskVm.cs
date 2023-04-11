using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModel
{
    public class CreateTaskVm
    {
        public List<SelectListItem> Developers { get; } = new List<SelectListItem>();


        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Required Hours")]
        public int RequiredHours { get; set; }

        [Display(Name ="Priority Level")]
        public Priority Priority { get; set; }

        public Project? Project { get; set; }
        public int ProjectId { get; set; }


        [Display(Name ="Completed")]
        public bool IsCompleted { get; set; } = false;


        [Display(Name = "Assignees")]
        public string UserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }


        public CreateTaskVm(HashSet<ApplicationUser> developer)
        {
            foreach (ApplicationUser u in developer)
            {
                Developers.Add(new SelectListItem(u.FullName, u.Id.ToString()));
            }
        }

        public CreateTaskVm() { }
    }
}
