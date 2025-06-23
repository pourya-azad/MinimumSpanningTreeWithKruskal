using System.ComponentModel.DataAnnotations;

namespace MinimumSpanningTreeWithKruskal.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر وارد کنید.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }
} 