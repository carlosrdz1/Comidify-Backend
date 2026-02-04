using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class Ingrediente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        public ICollection<ComidaIngrediente> ComidaIngredientes { get; set; } = new List<ComidaIngrediente>();
    }
}