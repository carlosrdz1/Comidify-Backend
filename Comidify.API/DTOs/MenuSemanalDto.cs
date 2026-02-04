using Comidify.API.Models;

namespace Comidify.API.DTOs
{
    public class MenuSemanalDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public List<MenuComidaDto> Comidas { get; set; } = new List<MenuComidaDto>();
    }

    public class CreateMenuSemanalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<CreateMenuComidaDto> Comidas { get; set; } = new List<CreateMenuComidaDto>();
    }

    public class UpdateMenuSemanalDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<CreateMenuComidaDto> Comidas { get; set; } = new List<CreateMenuComidaDto>();
    }

    public class MenuComidaDto
    {
        public int ComidaId { get; set; }
        public string NombreComida { get; set; } = string.Empty;
        public DiaSemana DiaSemana { get; set; }
        public TipoComida TipoComida { get; set; }
    }

    public class CreateMenuComidaDto
    {
        public int ComidaId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TipoComida TipoComida { get; set; }
    }

    public class ListaComprasDto
    {
        public List<IngredienteListaDto> Ingredientes { get; set; } = new List<IngredienteListaDto>();
    }

    public class IngredienteListaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public List<string> Cantidades { get; set; } = new List<string>();
    }
}