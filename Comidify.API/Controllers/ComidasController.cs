using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.Models;
using Comidify.API.DTOs;

namespace Comidify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComidasController : ControllerBase
    {
        private readonly ComidifyDbContext _context;

        public ComidasController(ComidifyDbContext context)
        {
            _context = context;
        }

        // GET: api/Comidas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComidaDto>>> GetComidas(
            [FromQuery] string? nombre = null,
            [FromQuery] TipoComida? tipoComida = null)
        {
            var query = _context.Comidas
                .Include(c => c.ComidaIngredientes)
                    .ThenInclude(ci => ci.Ingrediente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(c => c.Nombre.ToLower().Contains(nombre.ToLower()));
            }

            if (tipoComida.HasValue)
            {
                query = query.Where(c => c.TipoComida == tipoComida.Value);
            }

            var comidas = await query.ToListAsync();

            var result = comidas.Select(c => new ComidaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                TipoComida = c.TipoComida,
                Ingredientes = c.ComidaIngredientes.Select(ci => new IngredienteComidaDto
                {
                    IngredienteId = ci.IngredienteId,
                    NombreIngrediente = ci.Ingrediente.Nombre,
                    Cantidad = ci.Cantidad,
                    Unidad = ci.Unidad
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Comidas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComidaDto>> GetComida(int id)
        {
            var comida = await _context.Comidas
                .Include(c => c.ComidaIngredientes)
                    .ThenInclude(ci => ci.Ingrediente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comida == null)
            {
                return NotFound();
            }

            var result = new ComidaDto
            {
                Id = comida.Id,
                Nombre = comida.Nombre,
                TipoComida = comida.TipoComida,
                Ingredientes = comida.ComidaIngredientes.Select(ci => new IngredienteComidaDto
                {
                    IngredienteId = ci.IngredienteId,
                    NombreIngrediente = ci.Ingrediente.Nombre,
                    Cantidad = ci.Cantidad,
                    Unidad = ci.Unidad
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/Comidas
        [HttpPost]
        public async Task<ActionResult<ComidaDto>> CreateComida(CreateComidaDto dto)
        {
            var comida = new Comida
            {
                Nombre = dto.Nombre,
                TipoComida = dto.TipoComida,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Comidas.Add(comida);
            await _context.SaveChangesAsync();

            // Agregar ingredientes
            foreach (var ingredienteDto in dto.Ingredientes)
            {
                var comidaIngrediente = new ComidaIngrediente
                {
                    ComidaId = comida.Id,
                    IngredienteId = ingredienteDto.IngredienteId,
                    Cantidad = ingredienteDto.Cantidad,
                    Unidad = ingredienteDto.Unidad
                };
                _context.ComidaIngredientes.Add(comidaIngrediente);
            }

            await _context.SaveChangesAsync();

            // Cargar la comida completa con ingredientes
            var comidaCompleta = await _context.Comidas
                .Include(c => c.ComidaIngredientes)
                    .ThenInclude(ci => ci.Ingrediente)
                .FirstOrDefaultAsync(c => c.Id == comida.Id);

            var result = new ComidaDto
            {
                Id = comidaCompleta!.Id,
                Nombre = comidaCompleta.Nombre,
                TipoComida = comidaCompleta.TipoComida,
                Ingredientes = comidaCompleta.ComidaIngredientes.Select(ci => new IngredienteComidaDto
                {
                    IngredienteId = ci.IngredienteId,
                    NombreIngrediente = ci.Ingrediente.Nombre,
                    Cantidad = ci.Cantidad,
                    Unidad = ci.Unidad
                }).ToList()
            };

            return CreatedAtAction(nameof(GetComida), new { id = result.Id }, result);
        }

        // PUT: api/Comidas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComida(int id, UpdateComidaDto dto)
        {
            var comida = await _context.Comidas
                .Include(c => c.ComidaIngredientes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comida == null)
            {
                return NotFound();
            }

            comida.Nombre = dto.Nombre;
            comida.TipoComida = dto.TipoComida;

            // Eliminar ingredientes existentes
            _context.ComidaIngredientes.RemoveRange(comida.ComidaIngredientes);

            // Agregar nuevos ingredientes
            foreach (var ingredienteDto in dto.Ingredientes)
            {
                var comidaIngrediente = new ComidaIngrediente
                {
                    ComidaId = comida.Id,
                    IngredienteId = ingredienteDto.IngredienteId,
                    Cantidad = ingredienteDto.Cantidad,
                    Unidad = ingredienteDto.Unidad
                };
                _context.ComidaIngredientes.Add(comidaIngrediente);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Comidas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComida(int id)
        {
            var comida = await _context.Comidas.FindAsync(id);
            
            if (comida == null)
            {
                return NotFound();
            }

            _context.Comidas.Remove(comida);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}