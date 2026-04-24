using Scalar.AspNetCore;
using Serilog;
using Wholesale.Web.Infrastructure;

// Serilog bootstrap logger — uygulama başlamadan önce hataları yakalar
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((ctx, services, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithEnvironmentName()
           .WriteTo.Console()
           .WriteTo.File("logs/wholesale-.log", rollingInterval: RollingInterval.Day)
           .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

    builder.AddApplicationServices();
    builder.AddInfrastructureServices();
    builder.AddWebServices();

    var app = builder.Build();

    // Middleware pipeline
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseDeveloperExceptionPage();
    }

    app.UseExceptionHandler(options => { });

    app.UseCors();
    app.UseAuthentication();
    app.UseMiddleware<TenantResolutionMiddleware>();
    app.UseAuthorization();

    // Health check
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
        .WithTags("System")
        .AllowAnonymous();

    app.MapEndpoints(typeof(Program).Assembly);

    Log.Information("Wholesale API başlatılıyor — ortam: {Env}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılamadı");
}
finally
{
    Log.CloseAndFlush();
}
