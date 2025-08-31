namespace Example.Infrastructure.AccountService;

public record struct LoginDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
