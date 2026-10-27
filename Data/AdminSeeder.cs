
using AuthTask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        string? email = configuration["Admin:Email"];
        string? password = configuration["Admin:Password"];
        string? fullName = configuration["Admin:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin email and password are required.");
        }

        if (password.Length < 12)
        {
            throw new InvalidOperationException(
                "Admin password must be at least 12 characters.");
        }

        email = email.Trim().ToLowerInvariant();

        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AuthDbContext>();

        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<AppUser>>();

        var existingAdmin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existingAdmin != null)
        {
            if (existingAdmin.Role != "Admin")
            {
                throw new InvalidOperationException(
                    "Admin email is already used by another account.");
            }

            return;
        }

        AppUser admin = new AppUser
        {
            FullName = string.IsNullOrWhiteSpace(fullName)
                ? "Administrator"
                : fullName.Trim(),
            Email = email,
            Role = "Admin"
        };

        admin.PasswordHash = hasher.HashPassword(
            admin,
            password
        );

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}
