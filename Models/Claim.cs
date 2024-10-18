namespace PROGPart2.Models
{
    public class Claim
    {
        public int Id { get; set; } // This is your claim's unique identifier
        public string LecturerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string University { get; set; } = string.Empty;
        public string DocumentPath { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string LecturerEmail { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public string ProgrammeCoordinatorName { get; set; } = string.Empty;
    }
}
