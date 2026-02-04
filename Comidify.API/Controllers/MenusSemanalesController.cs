using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.Models;
using Comidify.API.DTOs;

namespace Comidify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusSemanalesController : ControllerBase
    {
        private readonly ComidifyDbContext _context;

        public MenusSemanalesController(ComidifyDbContext context)
        {
            _context = context;
        }

        // GET: api/MenusSemanales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuSemanalDto>>> GetMenusSemanales()
        {
            var menus = await _context.MenusSemanales
                .Include(m => m.MenuComidas)
                    .ThenInclude(mc => mc.Comida)
                .OrderByDescending(m => m.FechaCreacion)
                .ToListAsync();

            var result = menus.Select(m => new MenuSemanalDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                FechaCreacion = m.FechaCreacion,
                Comidas = m.MenuComidas.Select(mc => new MenuComidaDto
                {
                    ComidaId = mc.ComidaId,
                    NombreComida = mc.Comida.Nombre,
                    DiaSemana = mc.DiaSemana,
                    TipoComida = mc.TipoComida
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/MenusSemanales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuSemanalDto>> GetMenuSemanal(int id)
        {
            var menu = await _context.MenusSemanales
                .Include(m => m.MenuComidas)
                    .ThenInclude(mc => mc.Comida)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
            {
                return NotFound();
            }

            var result = new MenuSemanalDto
            {
                Id = menu.Id,
                Nombre = menu.Nombre,
                FechaCreacion = menu.FechaCreacion,
                Comidas = menu.MenuComidas.Select(mc => new MenuComidaDto
                {
                    ComidaId = mc.ComidaId,
                    NombreComida = mc.Comida.Nombre,
                    DiaSemana = mc.DiaSemana,
                    TipoComida = mc.TipoComida
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/MenusSemanales
        [HttpPost]
        public async Task<ActionResult<MenuSemanalDto>> CreateMenuSemanal(CreateMenuSemanalDto dto)
        {
            var menu = new MenuSemanal
            {
                Nombre = dto.Nombre,
                FechaCreacion = DateTime.UtcNow
            };

            _context.MenusSemanales.Add(menu);
            await _context.SaveChangesAsync();

            // Agregar comidas al menú
            foreach (var comidaDto in dto.Comidas)
            {
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

            // Cargar el menú completo con comidas
            var menuCompleto = await _context.MenusSemanales
                .Include(m => m.MenuComidas)
                    .ThenInclude(mc => mc.Comida)
                .FirstOrDefaultAsync(m => m.Id == menu.Id);

            var result = new MenuSemanalDto
            {
                Id = menuCompleto!.Id,
                Nombre = menuCompleto.Nombre,
                FechaCreacion = menuCompleto.FechaCreacion,
                Comidas = menuCompleto.MenuComidas.Select(mc => new MenuComidaDto
                {
                    ComidaId = mc.ComidaId,
                    NombreComida = mc.Comida.Nombre,
                    DiaSemana = mc.DiaSemana,
                    TipoComida = mc.TipoComida
                }).ToList()
            };

            return CreatedAtAction(nameof(GetMenuSemanal), new { id = result.Id }, result);
        }

        // PUT: api/MenusSemanales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuSemanal(int id, UpdateMenuSemanalDto dto)
        {
            var menu = await _context.MenusSemanales
                .Include(m => m.MenuComidas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
            {
                return NotFound();
            }

            menu.Nombre = dto.Nombre;

            // Eliminar comidas existentes
            _context.MenuComidas.RemoveRange(menu.MenuComidas);

            // Agregar nuevas comidas
            foreach (var comidaDto in dto.Comidas)
            {
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

            return NoContent();
        }

        // DELETE: api/MenusSemanales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuSemanal(int id)
        {
            var menu = await _context.MenusSemanales.FindAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            _context.MenusSemanales.Remove(menu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/MenusSemanales/5/lista-compras
        [HttpGet("{id}/lista-compras")]
        public async Task<ActionResult<ListaComprasDto>> GetListaCompras(int id)
        {
            var menu = await _context.MenusSemanales
                .Include(m => m.MenuComidas)
                    .ThenInclude(mc => mc.Comida)
                        .ThenInclude(c => c.ComidaIngredientes)
                            .ThenInclude(ci => ci.Ingrediente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menu == null)
            {
                return NotFound();
            }

            // Agrupar ingredientes
            var ingredientesAgrupados = menu.MenuComidas
                .SelectMany(mc => mc.Comida.ComidaIngredientes)
                .GroupBy(ci => ci.Ingrediente.Nombre)
                .Select(g => new IngredienteListaDto
                {
                    Nombre = g.Key,
                    Cantidades = g
                        .Where(ci => !string.IsNullOrEmpty(ci.Cantidad) || !string.IsNullOrEmpty(ci.Unidad))
                        .Select(ci => $"{ci.Cantidad ?? ""} {ci.Unidad ?? ""}".Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList()
                })
                .OrderBy(i => i.Nombre)
                .ToList();

            var result = new ListaComprasDto
            {
                Ingredientes = ingredientesAgrupados
            };

            return Ok(result);
        }
    }
}