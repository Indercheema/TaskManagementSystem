using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(200, ErrorMessage = "Project Title must be 5 to 200 characters long", MinimumLength = 5)]
        [Display(Name = "Project Title")]
        public string Title { get; set; }

   

        public virtual HashSet<ProjectContributor> ProjectContributors { get; set; } = new HashSet<ProjectContributor>();
        
        public virtual HashSet<Task> Tasks { get; set; } 

        public Project() { }

        public Project(string title)
        {
            Title = title;
        }
    }
}
