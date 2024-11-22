using Microsoft.AspNetCore.Mvc;
using Xunit;

public class ClaimTests
{
    [Fact]
    public void TestCreateClaim_ValidData_AddsClaimToDatabase()
    {
        // Arrange
        var mockContext = GetMockDbContext();
        var controller = new ClaimsController(mockContext);

        var claim = new Claim
        {
            Id = 1,
            LecturerEmail = "lecturer@example.com",
            HoursWorked = 10,
            HourlyRate = 200
        };

        // Act
        var result = controller.Create(claim);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Contains(claim, mockContext.Claims.ToList());
    }

    [Fact]
    public void TestEditClaim_ValidClaimId_UpdatesClaimInDatabase()
    {
        // Arrange
        var mockContext = GetMockDbContext();
        var controller = new ClaimsController(mockContext);

        var claim = mockContext.Claims.First();
        claim.HoursWorked = 20;

        // Act
        var result = controller.Edit(claim.Id, claim);

        // Assert
        var updatedClaim = mockContext.Claims.Find(claim.Id);
        Assert.Equal(20, updatedClaim.HoursWorked);
    }
}
