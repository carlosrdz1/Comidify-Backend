using System.ComponentModel.DataAnnotations;

namespace Comidify.API.Models
{
    public class MenuComida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MenuSemanalId { get; set; }
        public MenuSemanal MenuSemanal { get; set; } = null!;

        [Required]
        public int ComidaId { get; set; }
        public Comida Comida { get; set; } = null!;

        [Required]
        public DiaSemana DiaSemana { get; set; }

        [Required]
        public TipoComida TipoComida { get; set; }
    }
}