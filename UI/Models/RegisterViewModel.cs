using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.UI.Models
{
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "The role field is required.")]
    public string SelectedRole { get; set; }

    public List<string>? Roles { get; set; }
}



}
