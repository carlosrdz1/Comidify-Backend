using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.Models;
using Comidify.API.DTOs;
using Comidify.API.Extensions;

namespace Comidify.API.Controllers;

[Authorize] // ← NUEVO: Requiere autenticación
[ApiController]
[Route("api/[controller]")]
public class ComidasController : ControllerBase
{
    private readonly ComidifyDbContext _context;

    public ComidasController(ComidifyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComidaDto>>> GetComidas(
        [FromQuery] string? nombre,
        [FromQuery] int? tipoComida)
    {
        var userId = User.GetUserId(); // ← NUEVO

        var query = _context.Comidas
            .Where(c => c.UsuarioId == userId) // ← NUEVO: Solo del usuario
            .Include(c => c.Ingredientes)
                .ThenInclude(ci => ci.Ingrediente)
            .AsQueryable();

        if (!string.IsNullOrEmpty(nombre))
        {
            query = query.Where(c => c.Nombre.Contains(nombre));
        }

        if (tipoComida.HasValue)
        {
            query = query.Where(c => c.TipoComida == (TipoComida)tipoComida.Value);
        }

        var comidas = await query.ToListAsync();

        var comidasDto = comidas.Select(c => new ComidaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            TipoComida = c.TipoComida,
            Ingredientes = c.Ingredientes.Select(ci => new IngredienteEnComidaDto
            {
                IngredienteId = ci.IngredienteId,
                NombreIngrediente = ci.Ingrediente.Nombre,
                Cantidad = ci.Cantidad,
                Unidad = ci.Unidad
            }).ToList()
        }).ToList();

        return Ok(comidasDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComidaDto>> GetComida(int id)
    {
        var userId = User.GetUserId(); // ← NUEVO

        var comida = await _context.Comidas
            .Where(c => c.UsuarioId == userId) // ← NUEVO
            .Include(c => c.Ingredientes)
                .ThenInclude(ci => ci.Ingrediente)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comida == null)
        {
            return NotFound();
        }

        var comidaDto = new ComidaDto
        {
            Id = comida.Id,
            Nombre = comida.Nombre,
            TipoComida = comida.TipoComida,
            Ingredientes = comida.Ingredientes.Select(ci => new IngredienteEnComidaDto
            {
                IngredienteId = ci.IngredienteId,
                NombreIngrediente = ci.Ingrediente.Nombre,
                Cantidad = ci.Cantidad,
                Unidad = ci.Unidad
            }).ToList()
        };

        return Ok(comidaDto);
    }

    [HttpPost]
    public async Task<ActionResult<ComidaDto>> CreateComida(CreateComidaDto dto)
    {
        var userId = User.GetUserId(); // ← NUEVO

        var comida = new Comida
        {
            Nombre = dto.Nombre,
            TipoComida = dto.TipoComida,
            UsuarioId = userId // ← NUEVO
        };

        _context.Comidas.Add(comida);
        await _context.SaveChangesAsync();

        // Agregar ingredientes
        if (dto.Ingredientes != null && dto.Ingredientes.Any())
        {
            foreach (var ingredienteDto in dto.Ingredientes)
            {
                // Verificar que el ingrediente pertenezca al usuario
                var ingrediente = await _context.Ingredientes
                    .FirstOrDefaultAsync(i => i.Id == ingredienteDto.IngredienteId && i.UsuarioId == userId);

                if (ingrediente == null)
                {
                    return BadRequest($"Ingrediente {ingredienteDto.IngredienteId} no encontrado");
                }

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
        }

        // Recargar con ingredientes
        comida = await _context.Comidas
            .Include(c => c.Ingredientes)
                .ThenInclude(ci => ci.Ingrediente)
            .FirstAsync(c => c.Id == comida.Id);

        var comidaDto = new ComidaDto
        {
            Id = comida.Id,
            Nombre = comida.Nombre,
            TipoComida = comida.TipoComida,
            Ingredientes = comida.Ingredientes.Select(ci => new IngredienteEnComidaDto
            {
                IngredienteId = ci.IngredienteId,
                NombreIngrediente = ci.Ingrediente.Nombre,
                Cantidad = ci.Cantidad,
                Unidad = ci.Unidad
            }).ToList()
        };

        return CreatedAtAction(nameof(GetComida), new { id = comida.Id }, comidaDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComida(int id, CreateComidaDto dto)
    {
        var userId = User.GetUserId(); // ← NUEVO

        var comida = await _context.Comidas
            .Where(c => c.UsuarioId == userId) // ← NUEVO
            .Include(c => c.Ingredientes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comida == null)
        {
            return NotFound();
        }

        comida.Nombre = dto.Nombre;
        comida.TipoComida = dto.TipoComida;

        // Eliminar ingredientes existentes
        _context.ComidaIngredientes.RemoveRange(comida.Ingredientes);

        // Agregar nuevos ingredientes
        if (dto.Ingredientes != null && dto.Ingredientes.Any())
        {
            foreach (var ingredienteDto in dto.Ingredientes)
            {
                var ingrediente = await _context.Ingredientes
                    .FirstOrDefaultAsync(i => i.Id == ingredienteDto.IngredienteId && i.UsuarioId == userId);

                if (ingrediente == null)
                {
                    return BadRequest($"Ingrediente {ingredienteDto.IngredienteId} no encontrado");
                }

                var comidaIngrediente = new ComidaIngrediente
                {
                    ComidaId = comida.Id,
                    IngredienteId = ingredienteDto.IngredienteId,
                    Cantidad = ingredienteDto.Cantidad,
                    Unidad = ingredienteDto.Unidad
                };
                _context.ComidaIngredientes.Add(comidaIngrediente);
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComida(int id)
    {
        var userId = User.GetUserId(); // ← NUEVO

        var comida = await _context.Comidas
            .Where(c => c.UsuarioId == userId) // ← NUEVO
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comida == null)
        {
            return NotFound();
        }

        _context.Comidas.Remove(comida);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}