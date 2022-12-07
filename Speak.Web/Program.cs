using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Speak.Telegram.Bot;
using Speak.Telegram.Postgres;
using Speak.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.WebHost
    .ConfigureKestrel(o =>
        o.ListenAnyIP(443, options =>
        {
            options.UseHttps(d =>
                d.ServerCertificate = X509Certificate2.CreateFromPemFile("cert.crt", "private.key"));
        }));

// Регистрация сервисов
var allowedOrigings = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services
    .AddBot(builder.Configuration)
    .AddCors(options => options
        .AddPolicy("CorsPolicy", policyBuilder => policyBuilder
            .WithOrigins(allowedOrigings)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()))
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSignalR();

var app = builder.Build();

var dbContext = app.Services.GetRequiredService<TelegramBotDbContext>();
dbContext.Database.Migrate();

// Пайплайн HTTP реквестов
if (app.Environment.IsDevelopment()) app.UseSwagger().UseSwaggerUI();

app
    .UseHttpsRedirection()
    .UseRouting()
    .UseCors("CorsPolicy")
    .UseEndpoints(endpoints => endpoints
        .MapBotRoute(builder.Configuration)
        .MapControllers())
    .UseAuthorization()
    .UseDefaultFiles()
    .UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = context =>
        {
            var headers = context.Context.Response.GetTypedHeaders();
            headers.CacheControl = new() { Public = true, MaxAge = TimeSpan.FromMinutes(1) };
        }
    });

app.MapHub<WebRtcHub>("/signallingHub");
app.MapFallbackToFile("index.html");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершилось...");
}
finally
{
    Log.CloseAndFlush();
}
