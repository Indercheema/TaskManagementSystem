using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaskManagementSystem.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(200, ErrorMessage = "Task Title must be 5 to 200 characters long", MinimumLength = 5)]
        [Display(Name = "Task Title")]
        public string Title { get; set; }

        [Display(Name = "Required Hours")]
        public int RequiredHours { get; set; }

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        public Priority Priority { get; set; }

        public Project? Project { get; set; }

        public int ProjectId { get; set; }

        public virtual HashSet<TaskContributor> TaskContributors { get; set; } = new HashSet<TaskContributor>();

        public Task() { }

        public Task(string title, int requiredHours, bool isCompleted, Priority priority, Project project)
        {
            Title = title;
            RequiredHours = requiredHours;
            IsCompleted = isCompleted;
            Priority = priority;
            Project = project;
            ProjectId = project.Id;
        }
    }

    public enum Priority
    {
        Low = 1,
        Medium,
        High
    }
}
