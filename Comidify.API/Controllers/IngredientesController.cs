using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.Models;
using Comidify.API.DTOs;

namespace Comidify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientesController : ControllerBase
    {
        private readonly ComidifyDbContext _context;

        public IngredientesController(ComidifyDbContext context)
        {
            _context = context;
        }

        // GET: api/Ingredientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredienteDto>>> GetIngredientes(
            [FromQuery] string? nombre = null)
        {
            var query = _context.Ingredientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(i => i.Nombre.ToLower().Contains(nombre.ToLower()));
            }

            var ingredientes = await query
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            var result = ingredientes.Select(i => new IngredienteDto
            {
                Id = i.Id,
                Nombre = i.Nombre
            }).ToList();

            return Ok(result);
        }

        // GET: api/Ingredientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredienteDto>> GetIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            var result = new IngredienteDto
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre
            };

            return Ok(result);
        }

        // POST: api/Ingredientes
        [HttpPost]
        public async Task<ActionResult<IngredienteDto>> CreateIngrediente(CreateIngredienteDto dto)
        {
            // Verificar si ya existe un ingrediente con el mismo nombre
            var existente = await _context.Ingredientes
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existente != null)
            {
                return Conflict(new { message = "Ya existe un ingrediente con ese nombre" });
            }

            var ingrediente = new Ingrediente
            {
                Nombre = dto.Nombre,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();

            var result = new IngredienteDto
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre
            };

            return CreatedAtAction(nameof(GetIngrediente), new { id = result.Id }, result);
        }

        // PUT: api/Ingredientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngrediente(int id, UpdateIngredienteDto dto)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            // Verificar si ya existe otro ingrediente con el mismo nombre
            var existente = await _context.Ingredientes
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == dto.Nombre.ToLower() && i.Id != id);

            if (existente != null)
            {
                return Conflict(new { message = "Ya existe un ingrediente con ese nombre" });
            }

            ingrediente.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Ingredientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes
                .Include(i => i.ComidaIngredientes)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            // Verificar si el ingrediente está siendo usado en alguna comida
            if (ingrediente.ComidaIngredientes.Any())
            {
                return BadRequest(new { message = "No se puede eliminar el ingrediente porque está siendo usado en una o más comidas" });
            }

            _context.Ingredientes.Remove(ingrediente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}