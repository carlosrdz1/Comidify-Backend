using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        // NUEVO: Relación con Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Relaciones
        public ICollection<ComidaIngrediente> ComidaIngredientes { get; set; } = new List<ComidaIngrediente>();
    }
}