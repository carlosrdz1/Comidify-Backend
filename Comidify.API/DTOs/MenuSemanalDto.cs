using System.ComponentModel.DataAnnotations;
using Comidify.API.Models;

namespace Comidify.API.DTOs;

public class MenuSemanalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public List<MenuComidaDto> Comidas { get; set; } = new();
}

public class CreateMenuSemanalDto
{
    [Required]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    public List<CreateMenuComidaDto>? Comidas { get; set; }
}

// NUEVO DTO
public class MenuComidaDto
{
    public int ComidaId { get; set; }
    public string NombreComida { get; set; } = string.Empty;
    public DiaSemana DiaSemana { get; set; }
    public TipoComida TipoComida { get; set; }
}

// NUEVO DTO
public class CreateMenuComidaDto
{
    [Required]
    public int ComidaId { get; set; }

    [Required]
    public DiaSemana DiaSemana { get; set; }

    [Required]
    public TipoComida TipoComida { get; set; }
}