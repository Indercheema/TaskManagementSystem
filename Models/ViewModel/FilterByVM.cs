using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaskManagementSystem.Models.ViewModel
{
    public class FilterByVM
    {
        [Display(Name ="Filter By")]
        public FilterBy Filter { get; set; } = FilterBy.Hours;

        [Display(Name =("Order By"))]
        public OrderBy Order { get; set; } = OrderBy.Ascending;

        public Project Project { get; set; }

        public int ProjectId { get; set; }

        public Task Task { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public HashSet<Task> Tasks { get; set; } = new HashSet<Task>();

        public HashSet<Project> Projects { get; set; } = new HashSet<Project>();

    }

    public enum FilterBy
    {
        Hours = 1,
        Priority,
        [Display(Name = "Completed Task")]
        CompletedTask,
        [Display(Name = "Assigned Task")]
        AssignedTask
    }

    public enum OrderBy
    {
        Descending,
        Ascending
    }
}
