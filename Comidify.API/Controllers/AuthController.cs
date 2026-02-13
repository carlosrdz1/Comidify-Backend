using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;
using Comidify.API.DTOs;
using Comidify.API.Models;
using Comidify.API.Services;

namespace Comidify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ComidifyDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ComidifyDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        // Verificar si el email ya existe
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("El email ya está registrado");
        }

        // Crear usuario
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Generar token
        var token = _tokenService.CreateToken(usuario);

        return new AuthResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Token = token
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        // Buscar usuario
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (usuario == null)
        {
            return Unauthorized("Email o contraseña incorrectos");
        }

        // Verificar contraseña (solo si NO es OAuth)
        if (usuario.ProviderName == null)
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                return Unauthorized("Email o contraseña incorrectos");
            }
        }
        else
        {
            return BadRequest("Este usuario se registró con " + usuario.ProviderName);
        }

        // Generar token
        var token = _tokenService.CreateToken(usuario);

        return new AuthResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Token = token
        };
    }

    // ENDPOINT DE GOOGLE LOGIN (puede que no lo tengas)
    [HttpPost("google-login")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        try
        {
            // Buscar usuario existente por email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (usuario == null)
            {
                // Crear nuevo usuario
                usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Email = dto.Email.ToLower(),
                    ProviderName = "Google",
                    ProviderId = dto.ProviderId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Usuario existe, verificar que sea de Google o actualizar
                if (usuario.ProviderName == null)
                {
                    // Usuario se registró con email/password, vincular con Google
                    usuario.ProviderName = "Google";
                    usuario.ProviderId = dto.ProviderId;
                    await _context.SaveChangesAsync();
                }
            }

            var token = _tokenService.CreateToken(usuario);

            return new AuthResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Token = token
            };
        }
        catch (Exception ex)
        {
            return BadRequest("Error al autenticar con Google: " + ex.Message);
        }
    }
}