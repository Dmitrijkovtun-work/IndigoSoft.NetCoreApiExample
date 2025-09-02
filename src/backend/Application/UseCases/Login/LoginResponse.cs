namespace Example.Application.UseCases.Login;

public struct LoginResponse
{
    public required string UserName { get; set; }
    public string? BearerToken { get; set; }
}
