using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class Comida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public TipoComida TipoComida { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        public ICollection<ComidaIngrediente> ComidaIngredientes { get; set; } = new List<ComidaIngrediente>();
        public ICollection<MenuComida> MenuComidas { get; set; } = new List<MenuComida>();
    }
}