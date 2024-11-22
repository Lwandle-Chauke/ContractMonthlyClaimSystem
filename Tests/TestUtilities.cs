using Microsoft.EntityFrameworkCore;
using PROGPart2.Models;

public static class TestUtilities
{
    public static ApplicationDbContext GetMockDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed test data if necessary
        SeedTestData(context);

        return context;
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        // Add sample claims
        context.Claims.AddRange(new List<Claim>
        {
            new Claim { Id = 1, LecturerEmail = "lecturer@example.com", HoursWorked = 10, HourlyRate = 150 },
            new Claim { Id = 2, LecturerEmail = "lecturer@example.com", HoursWorked = 8, HourlyRate = 200 }
        });

        context.SaveChanges();
    }
}
