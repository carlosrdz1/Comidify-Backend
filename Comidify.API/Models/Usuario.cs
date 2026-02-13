using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models;

public class Usuario
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    // OAuth (Google/Facebook)
    public string? ProviderName { get; set; } // "Google", "Facebook", null
    public string? ProviderId { get; set; } // ID del usuario en Google/Facebook
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    // Relaciones
    public ICollection<Comida> Comidas { get; set; } = new List<Comida>();
    public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
    public ICollection<MenuSemanal> MenusSemanales { get; set; } = new List<MenuSemanal>();
}