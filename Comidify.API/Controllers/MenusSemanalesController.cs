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
public class MenusSemanalesController : ControllerBase
{
    private readonly ComidifyDbContext _context;

    public MenusSemanalesController(ComidifyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuSemanalDto>>> GetMenus()
    {
        var userId = User.GetUserId();

        var menus = await _context.MenusSemanales
            .Where(m => m.UsuarioId == userId)
            .Include(m => m.Comidas)
                .ThenInclude(mc => mc.Comida)
            .ToListAsync();

        var menusDto = menus.Select(m => new MenuSemanalDto
        {
            Id = m.Id,
            Nombre = m.Nombre,
            FechaCreacion = m.FechaCreacion,
            Comidas = m.Comidas.Select(mc => new MenuComidaDto
            {
                ComidaId = mc.ComidaId,
                NombreComida = mc.Comida.Nombre,
                DiaSemana = mc.DiaSemana,
                TipoComida = mc.TipoComida
            }).ToList()
        }).ToList();

        return Ok(menusDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuSemanalDto>> GetMenu(int id)
    {
        var userId = User.GetUserId();

        var menu = await _context.MenusSemanales
            .Where(m => m.UsuarioId == userId)
            .Include(m => m.Comidas)
                .ThenInclude(mc => mc.Comida)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
        {
            return NotFound();
        }

        var menuDto = new MenuSemanalDto
        {
            Id = menu.Id,
            Nombre = menu.Nombre,
            FechaCreacion = menu.FechaCreacion,
            Comidas = menu.Comidas.Select(mc => new MenuComidaDto
            {
                ComidaId = mc.ComidaId,
                NombreComida = mc.Comida.Nombre,
                DiaSemana = mc.DiaSemana,
                TipoComida = mc.TipoComida
            }).ToList()
        };

        return Ok(menuDto);
    }

    [HttpPost]
    public async Task<ActionResult<MenuSemanalDto>> CreateMenu(CreateMenuSemanalDto dto)
    {
        var userId = User.GetUserId();

        var menu = new MenuSemanal
        {
            Nombre = dto.Nombre,
            UsuarioId = userId
        };

        _context.MenusSemanales.Add(menu);
        await _context.SaveChangesAsync();

        if (dto.Comidas != null && dto.Comidas.Any())
        {
            foreach (var comidaDto in dto.Comidas)
            {
                // Verificar que la comida pertenezca al usuario
                var comida = await _context.Comidas
                    .FirstOrDefaultAsync(c => c.Id == comidaDto.ComidaId && c.UsuarioId == userId);

                if (comida == null)
                {
                    return BadRequest($"Comida {comidaDto.ComidaId} no encontrada");
                }

                var menuComida = new MenuComida
                {
                    MenuSemanalId = menu.Id,
                    ComidaId = comidaDto.ComidaId,
                    DiaSemana = comidaDto.DiaSemana,
                    TipoComida = comidaDto.TipoComida
                };
                _context.MenuComidas.Add(menuComida);
            }
            await _context.SaveChangesAsync();
        }

        // Recargar con comidas
        menu = await _context.MenusSemanales
            .Include(m => m.Comidas)
                .ThenInclude(mc => mc.Comida)
            .FirstAsync(m => m.Id == menu.Id);

        var menuDto = new MenuSemanalDto
        {
            Id = menu.Id,
            Nombre = menu.Nombre,
            FechaCreacion = menu.FechaCreacion,
            Comidas = menu.Comidas.Select(mc => new MenuComidaDto
            {
                ComidaId = mc.ComidaId,
                NombreComida = mc.Comida.Nombre,
                DiaSemana = mc.DiaSemana,
                TipoComida = mc.TipoComida
            }).ToList()
        };

        return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menuDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenu(int id, CreateMenuSemanalDto dto)
    {
        var userId = User.GetUserId();

        var menu = await _context.MenusSemanales
            .Where(m => m.UsuarioId == userId)
            .Include(m => m.Comidas)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
        {
            return NotFound();
        }

        menu.Nombre = dto.Nombre;

        // Eliminar comidas existentes
        _context.MenuComidas.RemoveRange(menu.Comidas);

        // Agregar nuevas comidas
        if (dto.Comidas != null && dto.Comidas.Any())
        {
            foreach (var comidaDto in dto.Comidas)
            {
                var comida = await _context.Comidas
                    .FirstOrDefaultAsync(c => c.Id == comidaDto.ComidaId && c.UsuarioId == userId);

                if (comida == null)
                {
                    return BadRequest($"Comida {comidaDto.ComidaId} no encontrada");
                }

                var menuComida = new MenuComida
                {
                    MenuSemanalId = menu.Id,
                    ComidaId = comidaDto.ComidaId,
                    DiaSemana = comidaDto.DiaSemana,
                    TipoComida = comidaDto.TipoComida
                };
                _context.MenuComidas.Add(menuComida);
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var userId = User.GetUserId();

        var menu = await _context.MenusSemanales
            .Where(m => m.UsuarioId == userId)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
        {
            return NotFound();
        }

        _context.MenusSemanales.Remove(menu);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/lista-compras")]
    public async Task<ActionResult<object>> GetListaCompras(int id)
    {
        var userId = User.GetUserId();

        var menu = await _context.MenusSemanales
            .Where(m => m.UsuarioId == userId)
            .Include(m => m.Comidas)
                .ThenInclude(mc => mc.Comida)
                    .ThenInclude(c => c.Ingredientes)
                        .ThenInclude(ci => ci.Ingrediente)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
        {
            return NotFound();
        }

        // Agrupar ingredientes
        var ingredientesAgrupados = menu.Comidas
            .SelectMany(mc => mc.Comida.Ingredientes)
            .GroupBy(ci => ci.Ingrediente.Nombre)
            .Select(g => new
            {
                nombre = g.Key,
                cantidades = g.Select(ci =>
                    !string.IsNullOrEmpty(ci.Cantidad) && !string.IsNullOrEmpty(ci.Unidad)
                        ? $"{ci.Cantidad} {ci.Unidad}"
                        : string.Empty
                ).Where(c => !string.IsNullOrEmpty(c)).ToList()
            })
            .OrderBy(i => i.nombre)
            .ToList();

        return Ok(new { ingredientes = ingredientesAgrupados });
    }
}