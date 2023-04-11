using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModel
{
    public class CreateProjectVm
    {
        public List<SelectListItem> Developers { get; } = new List<SelectListItem>();
       

        [Display(Name="Project Title")]
        public string Title { get; set; }


        [Display(Name ="Assignees")]
        public string UserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }


        public CreateProjectVm(HashSet<ApplicationUser> developer)
        {
            foreach(ApplicationUser u in developer )
            {
                Developers.Add(new SelectListItem(u.FullName, u.Id.ToString()));    
            }
        }

        public CreateProjectVm() { }


    }
}
