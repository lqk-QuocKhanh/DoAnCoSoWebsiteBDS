using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

using Serilog;

using VietPropEstate.Application;

using VietPropEstate.Application.Common.Interfaces;

using VietPropEstate.Infrastructure;

using VietPropEstate.Infrastructure.Persistence;

using VietPropEstate.Infrastructure.Services;

using VietPropEstate.WebAPI.Extensions;

using VietPropEstate.WebAPI.Hubs;

using VietPropEstate.WebAPI.Middleware;



Log.Logger = new LoggerConfiguration()

    .WriteTo.Console()

    .CreateBootstrapLogger();



try

{

    var builder = WebApplication.CreateBuilder(args);



    if (!builder.Environment.IsEnvironment("Testing"))

    {

        builder.Host.UseSerilog((context, _, configuration) =>

            configuration.ReadFrom.Configuration(context.Configuration));

    }



    builder.Services.AddApplication();

    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);



    builder.Services

        .AddWebApiIdentity(builder.Configuration)

        .AddWebApiAuthentication(builder.Configuration)

        .AddWebApiCors(builder.Configuration)

        .AddWebApiSwagger()

        .AddWebApiRateLimiting(builder.Configuration)

        .AddWebApiCompression();



    builder.Services.AddControllers();

    builder.Services.AddSession(options =>

    {

        options.IdleTimeout = TimeSpan.FromMinutes(30);

        options.Cookie.HttpOnly = true;

        options.Cookie.IsEssential = true;

    });



    builder.Services.AddSignalR(options =>

    {

        options.EnableDetailedErrors = builder.Environment.IsDevelopment();

        options.MaximumReceiveMessageSize = 32 * 1024;

        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);

        options.HandshakeTimeout = TimeSpan.FromSeconds(15);

        options.KeepAliveInterval = TimeSpan.FromSeconds(15);

    });



    builder.Services.AddScoped<IChatNotificationService, SignalRChatNotificationService>();



    var app = builder.Build();



    app.UseMiddleware<ExceptionHandlingMiddleware>();



    var enableSwagger = builder.Configuration.GetValue("EnableSwagger", app.Environment.IsDevelopment());

    if (enableSwagger)

    {

        app.UseSwagger();

        app.UseSwaggerUI(c =>

        {

            c.SwaggerEndpoint("/swagger/v1/swagger.json", "VietPropEstate API v1");

            c.RoutePrefix = "swagger";

            c.DocumentTitle = "VietPropEstate API";

        });

        app.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription()
            .AllowAnonymous();

        Log.Information("Swagger UI: {SwaggerUrl}", "/swagger");

    }



    app.UseResponseCompression();

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
    Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "properties"));

    app.UseCors("BlazorUI");

    app.UseSession();

    if (!app.Environment.IsEnvironment("Testing"))
        app.UseSerilogRequestLogging();

    app.UseRateLimiter();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.MapHub<ChatHub>("/hubs/chat");



    using (var scope = app.Services.CreateScope())

    {

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (app.Environment.IsEnvironment("Testing"))
            await db.Database.EnsureCreatedAsync();
        else
            await db.Database.MigrateAsync();

    }



    var roleSeeder = app.Services.GetRequiredService<RoleSeeder>();

    await roleSeeder.SeedAsync();



    if (!app.Environment.IsEnvironment("Testing"))

    {

        var addressSeeder = app.Services.GetRequiredService<AddressDataSeeder>();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        await addressSeeder.SeedAsync(cts.Token);



        var testSeeder = app.Services.GetRequiredService<TestDataSeeder>();

        await testSeeder.SeedAsync();

    }



    app.Run();

}

catch (IOException ex) when (ex.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase)
    || (ex.InnerException?.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase) ?? false))
{
    Log.Fatal("Port 5198 is already in use. Stop the existing WebAPI instance first:");
    Log.Fatal("  .\\scripts\\stop-dev-ports.ps1");
    Log.Fatal("Or run: .\\scripts\\run-webapi.ps1");
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}

finally

{

    if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

        ?.Equals("Testing", StringComparison.OrdinalIgnoreCase) ?? false)

        Log.CloseAndFlush();

}

public partial class Program { }


