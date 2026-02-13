using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class Comida
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        public TipoComida TipoComida { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        // NUEVO: Relación con Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Relaciones
        public ICollection<ComidaIngrediente> Ingredientes { get; set; } = new List<ComidaIngrediente>();
        public ICollection<MenuComida> MenuComidas { get; set; } = new List<MenuComida>();
    }
}