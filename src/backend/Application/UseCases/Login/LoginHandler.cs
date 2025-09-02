using Example.Infrastructure.AccountService;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Example.Application.UseCases.Login;

public class LoginHandler(IConfiguration conf) : IRequestHandler<LoginRequest, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        
        var secret = conf["secret"];

        return Task.FromResult(new LoginResponse
        {
            UserName = request.UserName,
            BearerToken = GenerateJwtToken(request.UserName, secret ?? "")
        });
    }


    private static string GenerateJwtToken(string userName, string secret)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "IndigoSoft.ExampleApi.Service",
            audience: "",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
