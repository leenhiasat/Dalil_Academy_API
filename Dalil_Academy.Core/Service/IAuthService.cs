
using Dalil_Academy.Core.DTOs.Auth;

namespace Dalil_Academy.Core.Service;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
}
