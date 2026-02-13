using Comidify.API.Models;

namespace Comidify.API.Services;

public interface ITokenService
{
    string CreateToken(Usuario usuario);
}