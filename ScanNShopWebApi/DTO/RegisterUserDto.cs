using System.ComponentModel.DataAnnotations;

namespace ScanNShopWebApi.DTO
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // ❗ Kein PasswordHash mehr!
    }

}
