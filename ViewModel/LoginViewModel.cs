using System.ComponentModel.DataAnnotations;

namespace MinimumSpanningTreeWithKruskal.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر وارد کنید.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
} 