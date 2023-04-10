using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class TaskContributor
    {
        public int Id { get; set; }

        public Task? Task { get; set; }

        public int TaskId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public string UserId { get; set; }

        public TaskContributor() { }

        public TaskContributor(Task task, ApplicationUser applicationUser)
        {
            Task = task;
            TaskId = task.Id;
            ApplicationUser = applicationUser;
            UserId = applicationUser.Id;
        }
    }
}
