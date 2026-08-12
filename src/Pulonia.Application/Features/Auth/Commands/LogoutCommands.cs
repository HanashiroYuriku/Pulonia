using Pulonia.Application.Common.Interfaces.Authentication;
using Pulonia.Application.Common.Interfaces.Repositories;
using Pulonia.Domain.Exceptions;
using Cortex.Mediator.Commands;
using FluentValidation;

namespace Pulonia.Application.Features.Auth.Commands;

public record LogoutCommand(Guid UserId) : ICommand;

public class LogoutHandler : ICommandHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDbTransactionManager _txManager;

    public LogoutHandler(IUserRepository userRepository, IDbTransactionManager txManager)
    {
        _userRepository = userRepository;
        _txManager = txManager;
    }

    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken, trackChanges: true);

        if (user != null)
        {
            user.RevokeRefreshToken();
            _userRepository.Update(user);

            await _txManager.SaveChangesAsync(cancellationToken);
        }
    }
}