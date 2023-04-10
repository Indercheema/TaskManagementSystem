using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models
{
    public class ProjectContributor
    {
        public int Id { get; set; }

        public Project? Project { get; set; }

        public int ProjectId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public string UserId { get; set; }

        public ProjectContributor()
        {

        }

        public ProjectContributor( Project project, ApplicationUser applicationUser)
        {
            Project = project;
            ProjectId = project.Id;
            ApplicationUser = applicationUser;
            UserId = applicationUser.Id;
        }
    }
}
