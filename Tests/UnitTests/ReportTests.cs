using Microsoft.AspNetCore.Mvc;
using PROGPart2.Controllers;
using Xunit;

public class ReportTests
{
    [Fact]
    public void TestGenerateClaimsReport_ValidLecturer_ReturnsValidReport()
    {
        // Arrange
        var mockContext = GetMockDbContext();
        var controller = new HRController(mockContext);

        // Act
        var result = controller.GenerateClaimsReportPdf();

        // Assert
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", result.ContentType);
    }

    [Fact]
    public void TestGenerateLecturerActivityReport_ValidData_ReturnsValidReport()
    {
        // Arrange
        var mockContext = GetMockDbContext();
        var controller = new HRController(mockContext);

        // Act
        var result = controller.GenerateLecturerActivityReportPdf();

        // Assert
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", result.ContentType);
    }
}
