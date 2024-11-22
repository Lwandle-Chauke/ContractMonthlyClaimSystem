using System.ComponentModel.DataAnnotations;

namespace PROGPart2.Models.ViewModels
{
    public class UpdateClaimStatusViewModel
    {
        public int ClaimId { get; set; }

        [Required]
        public string LecturerName { get; set; } = string.Empty;

        [Required]
        public string CurrentStatus { get; set; } = string.Empty;

        [Required]
        public List<string> StatusOptions { get; set; } = new List<string>(); // Ensure this is initialized

        [Required(ErrorMessage = "Please select a status.")]
        public string SelectedStatus { get; set; } = string.Empty;
    }
}
