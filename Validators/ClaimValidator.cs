using FluentValidation;
using System.IO;
using System.Linq;
using PROGPart2.Models; // Include the namespace where Claim is defined

public class ClaimValidator : AbstractValidator<Claim>
{
    public ClaimValidator()
    {
        RuleFor(c => c.HoursWorked).GreaterThan(0).WithMessage("Hours worked must be greater than zero.");
        RuleFor(c => c.HourlyRate).GreaterThan(0).WithMessage("Hourly rate must be greater than zero.");
        RuleFor(c => c.StartDate).LessThan(c => c.EndDate).WithMessage("Start date must be before end date.");

        RuleFor(c => c.DocumentPath)
            .NotEmpty().WithMessage("Document is required.")
            .Must(HaveValidExtension).WithMessage("Invalid document format.")
            .Must(FileExists).WithMessage("Document does not exist.");

        RuleFor(c => CalculateTotalAmount(c))
            .GreaterThan(0).WithMessage("Total amount must be greater than zero.");
    }

    // Validate the file extension
    private bool HaveValidExtension(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
        return allowedExtensions.Contains(Path.GetExtension(filePath).ToLower());
    }

    // Check if the file actually exists
    private bool FileExists(string filePath)
    {
        return File.Exists(filePath); // This checks if the file exists on the disk
    }

    // Calculate the total amount (to use in validation)
    private decimal CalculateTotalAmount(Claim claim)
    {
        return claim.HoursWorked * claim.HourlyRate;
    }
}
