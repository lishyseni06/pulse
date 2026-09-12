using System.ComponentModel.DataAnnotations;

namespace Pulse.Models;

public class Admin
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string PasswordHash { get; set; } = string.Empty;
}
