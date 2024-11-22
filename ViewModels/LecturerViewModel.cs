namespace PROGPart2.Models.ViewModels
{
    public class LecturerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int HoursWorked { get; set; } // Total hours worked
    }
}
