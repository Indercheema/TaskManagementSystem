using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Areas.Identity.Data;
using TaskManagementSystem.Data;

namespace TaskManagementSystem.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            TaskContext context = new TaskContext(serviceProvider.GetRequiredService<DbContextOptions<TaskContext>>());

            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!context.Roles.Any())
            {
                List<string> roles = new List<string> { "Administrator", "Developer", "Project Manager" };

                foreach (string r in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(r));
                }

                await context.SaveChangesAsync();
            }

            if (!context.Users.Any(u => u.UserName == "admin34@gmail.com"))
            {
                ApplicationUser defaultAdministrator = new ApplicationUser
                {
                    Email = "admin34@gmail.com",
                    NormalizedEmail = "ADMIN34@GMAIL.COM",
                    UserName = "admin34@gmail.com",
                    NormalizedUserName = "ADMIN34@GMAIL.COM",
                    EmailConfirmed = true
                };

                PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
                string HashedPassword = passwordHasher.HashPassword(defaultAdministrator, "Admin@34");
                defaultAdministrator.PasswordHash = HashedPassword;

                await userManager.CreateAsync(defaultAdministrator);
                await userManager.AddToRoleAsync(defaultAdministrator, "Administrator");
            }

            await context.SaveChangesAsync();
        }
    }
}
