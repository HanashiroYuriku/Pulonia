using Pulonia.Application.Features.Auth.Commands;
using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    // Generate token interface
    AuthResult GenerateToken(User user);
}