using System.ComponentModel.DataAnnotations;

namespace PROGPart2.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Assigns an empty string as the default value

        public string Role { get; set; } = string.Empty; // Assigns an empty string as the default value
    }
}
