using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Models;
using Project = TaskManagementSystem.Models.Project;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Data;

public class TaskContext : IdentityDbContext<ApplicationUser>
{
    public TaskContext(DbContextOptions<TaskContext> options)
        : base(options)
    {
    }

    public DbSet<Task> Task { get; set; } = default!;
    public DbSet<Project> Project { get; set; } = default!;

    public DbSet<ProjectContributor> ProjectContributor { get; set; } = default!;

    public DbSet<TaskContributor> TaskContributor { get; set; } = default!;

    private void _createTaskModel(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Task>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Project>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Projects)
            .WithOne(p => p.Manager)
            .HasForeignKey(p => p.MangerId);

        modelBuilder.Entity<Task>()
            .HasMany(t => t.TaskContributors)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId);


        modelBuilder.Entity<Project>()
            .HasMany(p => p.ProjectContributors)
            .WithOne(pc => pc.Project)
            .HasForeignKey(pc => pc.ProjectId);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(tc => tc.TaskContributors)
            .WithOne(au => au.ApplicationUser)
            .HasForeignKey(au => au.UserId);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(pc => pc.ProjectContributors)
            .WithOne(au => au.ApplicationUser)
            .HasForeignKey(au => au.UserId);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        _createTaskModel(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

}
