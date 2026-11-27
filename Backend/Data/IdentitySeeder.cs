
using EmployeeProjectAllocation.Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeProjectAllocation.Backend.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            // Create HR Admin role
            const string roleName = "HR Admin";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            // Read initial admin credentials from configuration
            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Initial admin credentials are missing.");
            }

            // Create admin user if it doesn't exist
            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Assign HR Admin role
            if (!await userManager.IsInRoleAsync(admin, roleName))
            {
                var result = await userManager.AddToRoleAsync(admin, roleName);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
