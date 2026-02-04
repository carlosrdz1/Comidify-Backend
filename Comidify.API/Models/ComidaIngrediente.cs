using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class ComidaIngrediente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ComidaId { get; set; }
        public Comida Comida { get; set; } = null!;

        [Required]
        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; } = null!;

        [StringLength(50)]
        public string? Cantidad { get; set; }

        [StringLength(50)]
        public string? Unidad { get; set; }
    }
}