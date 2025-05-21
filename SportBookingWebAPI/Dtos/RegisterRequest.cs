using System.ComponentModel.DataAnnotations;

namespace SportBookingWebAPI.Dtos
{
	public class RegisterRequest
	{
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
