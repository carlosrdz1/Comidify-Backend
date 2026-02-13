using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.Models;
using Comidify.API.DTOs;
using Comidify.API.Extensions;

namespace Comidify.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IngredientesController : ControllerBase
{
    private readonly ComidifyDbContext _context;

    public IngredientesController(ComidifyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredienteDto>>> GetIngredientes([FromQuery] string? nombre)
    {
        var userId = User.GetUserId();

        var query = _context.Ingredientes
            .Where(i => i.UsuarioId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(nombre))
        {
            query = query.Where(i => i.Nombre.Contains(nombre));
        }

        var ingredientes = await query.ToListAsync();

        var ingredientesDto = ingredientes.Select(i => new IngredienteDto
        {
            Id = i.Id,
            Nombre = i.Nombre
        }).ToList();

        return Ok(ingredientesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredienteDto>> GetIngrediente(int id)
    {
        var userId = User.GetUserId();

        var ingrediente = await _context.Ingredientes
            .Where(i => i.UsuarioId == userId)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingrediente == null)
        {
            return NotFound();
        }

        return Ok(new IngredienteDto { Id = ingrediente.Id, Nombre = ingrediente.Nombre });
    }

    [HttpPost]
    public async Task<ActionResult<IngredienteDto>> CreateIngrediente(CreateIngredienteDto dto)
    {
        var userId = User.GetUserId();

        var ingrediente = new Ingrediente
        {
            Nombre = dto.Nombre,
            UsuarioId = userId
        };

        _context.Ingredientes.Add(ingrediente);
        await _context.SaveChangesAsync();

        var ingredienteDto = new IngredienteDto
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre
        };

        return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, ingredienteDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngrediente(int id, CreateIngredienteDto dto)
    {
        var userId = User.GetUserId();

        var ingrediente = await _context.Ingredientes
            .Where(i => i.UsuarioId == userId)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingrediente == null)
        {
            return NotFound();
        }

        ingrediente.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngrediente(int id)
    {
        var userId = User.GetUserId();

        var ingrediente = await _context.Ingredientes
            .Where(i => i.UsuarioId == userId)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingrediente == null)
        {
            return NotFound();
        }

        var enUso = await _context.ComidaIngredientes.AnyAsync(ci => ci.IngredienteId == id);

        if (enUso)
        {
            return BadRequest("No se puede eliminar el ingrediente porque está siendo usado en una o más comidas");
        }

        _context.Ingredientes.Remove(ingrediente);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("test")]
    public async Task<ActionResult<IngredienteDto>> TestCreateIngrediente(CreateIngredienteDto dto)
    {
        // Hardcodear userId en lugar de sacarlo del token
        var userId = 1;

        var ingrediente = new Ingrediente
        {
            Nombre = dto.Nombre,
            UsuarioId = userId
        };

        _context.Ingredientes.Add(ingrediente);
        await _context.SaveChangesAsync();

        var ingredienteDto = new IngredienteDto
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre
        };

        return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, ingredienteDto);
    }
}