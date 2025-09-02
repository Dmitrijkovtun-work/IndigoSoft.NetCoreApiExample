using Example.Infrastructure.AccountService;
using MediatR;

namespace Example.Application.UseCases.Login;

public record LoginRequest: IRequest<LoginResponse>
{
    public required string UserName { get; set; }
    public required string HashPassword { get; set; }
}
