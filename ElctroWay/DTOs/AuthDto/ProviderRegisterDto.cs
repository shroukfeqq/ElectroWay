using System.ComponentModel.DataAnnotations;

namespace ElctroWay.DTOs.AuthDto
{
    public class ProviderRegisterDto
    {
        [MinLength(10, ErrorMessage = "Name must be at least 10 characters")]
        public string FullName { get; set; }

        public string Email { get; set; }
        
      
        public string Password { get; set; }
        
        [Compare("Password",
          ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^01[0-2,5][0-9]{8}$",
        ErrorMessage = "Invalid Egyptian phone number")]
        
        public string PhoneNumber { get; set; }

        

        public IFormFile FrontId { get; set; }

        public IFormFile BackId { get; set; }

        public IFormFile SelfieWithId { get; set; }
    }
}
