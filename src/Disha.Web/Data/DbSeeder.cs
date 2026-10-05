using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Disha.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DishaDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DishaDbContext>>();

            try
            {
                await context.Database.EnsureCreatedAsync();

                // If careers exist, database is already seeded
                if (await context.Careers.AnyAsync())
                {
                    return;
                }

                logger.LogInformation("Database is empty. Executing seed.sql...");
                string seedSqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "seed.sql");
                if (!File.Exists(seedSqlPath))
                {
                    seedSqlPath = Path.Combine(Directory.GetCurrentDirectory(), "seed.sql");
                }

                if (File.Exists(seedSqlPath))
                {
                    string script = await File.ReadAllTextAsync(seedSqlPath);
                    // Filter out USE disha_db statement if EF is connected directly
                    script = script.Replace("USE disha_db;", "", StringComparison.OrdinalIgnoreCase);
                    
                    var sqlStatements = script.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var statement in sqlStatements)
                    {
                        var trimmed = statement.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("--"))
                        {
                            try
                            {
                                await context.Database.ExecuteSqlRawAsync(trimmed);
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning("Seed SQL statement warning: {Message}", ex.Message);
                            }
                        }
                    }
                    logger.LogInformation("Database seeded successfully via seed.sql!");
                }
                else
                {
                    logger.LogWarning("seed.sql file not found at path: {Path}", seedSqlPath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
