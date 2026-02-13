using System.ComponentModel.DataAnnotations;
using Comidify.API.Models;

namespace Comidify.API.DTOs;

public class ComidaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoComida TipoComida { get; set; }
    public List<IngredienteEnComidaDto> Ingredientes { get; set; } = new();
}

public class CreateComidaDto
{
    [Required]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public TipoComida TipoComida { get; set; }

    public List<CreateIngredienteEnComidaDto>? Ingredientes { get; set; }
}

// NUEVO DTO
public class IngredienteEnComidaDto
{
    public int IngredienteId { get; set; }
    public string NombreIngrediente { get; set; } = string.Empty;
    public string? Cantidad { get; set; }
    public string? Unidad { get; set; }
}

// NUEVO DTO
public class CreateIngredienteEnComidaDto
{
    [Required]
    public int IngredienteId { get; set; }

    public string? Cantidad { get; set; }
    public string? Unidad { get; set; }
}