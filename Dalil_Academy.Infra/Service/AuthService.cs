using System.Security.Claims;
using System.Text;
using Dalil_Academy.Core.DTOs.Auth;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Core.Service;
using Dalil_Academy.Infra.Security;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Dalil_Academy.Infra.Service;

public class AuthService(
    IUserRepository users,
    IValidator<RegisterRequestDto> validator,
    IOptions<JwtSetting> jwtSettings) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        await validator.ValidateAndThrowAsync(dto);

        var email = dto.Email.Trim().ToLowerInvariant();
        var existingUser = await users.GetByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new ConflictException("This email is already registered.");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Roles.Student
        };

        var student = new Student
        {
            FullName = dto.FullName.Trim(),
            Email = email
        };

        await users.CreateAsync(user, student);
        return GenerateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return GenerateToken(user);
    }

    private AuthResponseDto GenerateToken(User user)
    {
        var settings = jwtSettings.Value;
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(settings.ExpiresInHours), 
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new AuthResponseDto(token);
    }
}