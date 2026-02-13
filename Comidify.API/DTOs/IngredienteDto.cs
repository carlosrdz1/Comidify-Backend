using System.ComponentModel.DataAnnotations;

namespace Comidify.API.DTOs;

public class IngredienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CreateIngredienteDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
}