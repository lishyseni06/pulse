using System.ComponentModel.DataAnnotations;

namespace Pulse.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Shkruani përdoruesin.")]
    [Display(Name = "Përdoruesi")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Shkruani fjalëkalimin.")]
    [DataType(DataType.Password)]
    [Display(Name = "Fjalëkalimi")]
    public string Password { get; set; } = string.Empty;
}
