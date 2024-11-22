using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // For IFormFile
using PROGPart2.Models;

namespace PROGPart2.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Hours Worked is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hours Worked must be greater than 0.")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly Rate must be greater than 0.")]
        public decimal HourlyRate { get; set; }

        public string University { get; set; } = string.Empty;
        public string DocumentPath { get; set; } = string.Empty;
        public string LecturerEmail { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public string ProgrammeCoordinatorName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Default value

        [Required(ErrorMessage = "Claim Document is required.")]
        [NotMapped] // This prevents EF Core from attempting to map the IFormFile property to the database
        public IFormFile ClaimDocument { get; set; }
    }
}
