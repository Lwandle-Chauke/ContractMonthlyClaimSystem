using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PROGPart2.Controllers;
using PROGPart2.Models;
using Microsoft.EntityFrameworkCore;

public class ClaimWorkflowIntegrationTests
{
    [Fact]
    public async Task TestClaimSubmissionAndRetrieval_EndToEnd_Success()
    {
        // Arrange
        var mockContext = TestUtilities.GetMockDbContext();
        var controller = new ClaimController(mockContext);

        var claim = new Claim
        {
            Id = 3,
            LecturerEmail = "lecturer@example.com",
            HoursWorked = 8,
            HourlyRate = 150
        };

        // Act
        var createResult = await controller.Create(claim) as RedirectToActionResult;
        var claims = await mockContext.Claims
            .Where(c => c.LecturerEmail == "lecturer@example.com")
            .ToListAsync();

        // Assert
        Assert.NotNull(createResult);
        Assert.Equal("Index", createResult.ActionName);
        Assert.Contains(claim, claims);
    }
}
