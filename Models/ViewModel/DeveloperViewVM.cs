namespace TaskManagementSystem.Models.ViewModel
{
    public class DeveloperViewVM
    {
        public HashSet<Task> Tasks { get; set; }

        public HashSet<ProjectContributor> ProjectContributors { get; set; }
    }
}
