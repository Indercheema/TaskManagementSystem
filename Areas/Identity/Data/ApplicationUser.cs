using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required(AllowEmptyStrings =false)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    public virtual HashSet<ProjectContributor> ProjectContributors { get; set; } = new HashSet<ProjectContributor>();

    public virtual HashSet<TaskContributor> TaskContributors { get; set; } = new HashSet<TaskContributor>();

}

