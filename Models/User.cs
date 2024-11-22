using System.ComponentModel.DataAnnotations;

namespace PROGPart2.Models
{
    public class User
    {
        [Key] // Specifies that Id is the primary key of the User entity
        public int Id { get; set; }

        [Required] // Marks Name as a required field
        public string Name { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required] // Marks Surname as a required field
        public string Surname { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required, EmailAddress] // Marks Email as required and ensures it is a valid email format
        public string Email { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required] // Marks PhoneNumber as a required field
        public string PhoneNumber { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty; // Assigns an empty string as the default value

        [Required] // Marks Role as a required field
        public string Role { get; set; } = string.Empty; // Assigns an empty string as the default value, expecting roles like Lecturer, Programme Coordinator, or Academic Manager

        // Additional properties specific to lecturers
        public int? HoursWorked { get; set; } // Nullable to keep it optional for non-lecturer roles
    }
}