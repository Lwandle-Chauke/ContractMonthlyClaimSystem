using Xunit;
using Microsoft.AspNetCore.Mvc;
using PROGPart2.Controllers;
using PROGPart2.Models;

public class ReportWorkflowIntegrationTests
{
    [Fact]
    public void TestClaimsReportDownload_ReturnsCSVFileWithData()
    {
        // Arrange
        var mockContext = TestUtilities.GetMockDbContext();
        var controller = new HRController(mockContext);

        // Act
        var result = controller.DownloadClaimsReport() as FileContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("text/csv", result.ContentType);

        var csvContent = System.Text.Encoding.UTF8.GetString(result.FileContents);
        Assert.Contains("Claim ID,Start Date,End Date,Hours Worked,Hourly Rate,Total Amount", csvContent);
        Assert.Contains("R", csvContent); // Check for formatted currency
    }
}
