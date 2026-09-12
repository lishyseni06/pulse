using System.ComponentModel.DataAnnotations;

namespace Pulse.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Emri i kategorisë është i detyrueshëm.")]
    [StringLength(50)]
    [Display(Name = "Emri")]
    public string Name { get; set; } = string.Empty;

    public ICollection<News> News { get; set; } = new List<News>();
}
