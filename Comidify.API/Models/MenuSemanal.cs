using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class MenuSemanal
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        // NUEVO: Relación con Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Relaciones
        public ICollection<MenuComida> Comidas { get; set; } = new List<MenuComida>();
    }
}