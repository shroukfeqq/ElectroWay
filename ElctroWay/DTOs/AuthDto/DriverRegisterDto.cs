using System.ComponentModel.DataAnnotations;

namespace ElctroWay.DTOs.AuthDto
{
    public class DriverRegisterDto
    {
        [Required]
        [MinLength(10, ErrorMessage = "Name must be at least 10 characters")]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^01[0-2,5][0-9]{8}$",
       ErrorMessage = "Invalid Egyptian phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password",
            ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
