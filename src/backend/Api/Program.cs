using Example.Application.DTOs;
using Example.Application.Services;
using Example.Domain.Services;
using Example.Infrastructure.LogService;
using Serilog;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

namespace Example.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(a => a.ExampleSink())
            .CreateBootstrapLogger();
        
        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(
            c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "IndigoSoft test API",
                Version = "v1",
                Description = "Test API"
            }));
        
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Async(a => a.ExampleSink())
        );
        RegisteringCustomServices(builder.Services);

        var app = builder.Build();
        
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    static void RegisteringCustomServices(IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateBounded<IncomeSlicePacket>(1));
        services.AddHostedService<SlicesProducer>();
        services.AddHostedService<SliceCollector>();
        services.AddSingleton<ExampleTaskPlanner>();
        services.AddSingleton<SliceProcessor>();
    }
}
