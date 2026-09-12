using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulse.Models;

public class News
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titulli është i detyrueshëm.")]
    [StringLength(200)]
    [Display(Name = "Titulli")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Përmbledhje")]
    [StringLength(400)]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Përmbajtja është e detyrueshme.")]
    [Display(Name = "Përmbajtja")]
    public string Content { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Foto (URL)")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Data e publikimit")]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    [Display(Name = "Kategoria")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}
