using System.ComponentModel.DataAnnotations;

namespace PINrad.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public required string ConfirmPassword { get; set; }

        [Required]
        public required string PunoIme { get; set; }

        [Required]
        public required string Rola { get; set; }
    }

}
