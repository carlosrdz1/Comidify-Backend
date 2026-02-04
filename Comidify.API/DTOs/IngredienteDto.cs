namespace Comidify.API.DTOs
{
    public class IngredienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class CreateIngredienteDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class UpdateIngredienteDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}