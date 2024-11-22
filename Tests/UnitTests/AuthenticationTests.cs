using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Controllers;
using PROGPart2.Models;
using PROGPart2.ViewModels;

public class AuthenticationTests
{
    [Fact]
    public void TestUserLogin_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var mockContext = new Mock<ApplicationDbContext>();
        var controller = new AccountController(mockContext.Object);

        // Act
        var result = controller.Login(new LoginViewModel { Email = "user@example.com", Password = "password123" });

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Dashboard", redirectResult.ActionName);
    }

    [Fact]
    public void TestUserLogin_InvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var mockContext = new Mock<ApplicationDbContext>();
        var controller = new AccountController(mockContext.Object);

        // Act
        var result = controller.Login(new LoginViewModel { Email = "invalid@example.com", Password = "wrongpassword" });

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Contains("Invalid login attempt", viewResult.ViewData["ErrorMessage"].ToString());
    }
}
