using Comidify.API.Models;

namespace Comidify.API.DTOs
{
    public class ComidaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoComida TipoComida { get; set; }
        public List<IngredienteComidaDto> Ingredientes { get; set; } = new List<IngredienteComidaDto>();
    }

    public class CreateComidaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public TipoComida TipoComida { get; set; }
        public List<IngredienteComidaCreateDto> Ingredientes { get; set; } = new List<IngredienteComidaCreateDto>();
    }

    public class UpdateComidaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public TipoComida TipoComida { get; set; }
        public List<IngredienteComidaCreateDto> Ingredientes { get; set; } = new List<IngredienteComidaCreateDto>();
    }

    public class IngredienteComidaDto
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public string? Cantidad { get; set; }
        public string? Unidad { get; set; }
    }

    public class IngredienteComidaCreateDto
    {
        public int IngredienteId { get; set; }
        public string? Cantidad { get; set; }
        public string? Unidad { get; set; }
    }
}