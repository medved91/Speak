using System.Security.Cryptography.X509Certificates;
using Speak.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => 
    o.ListenAnyIP(443, options =>
    {
        options.UseHttps(d => 
            d.ServerCertificate = X509Certificate2.CreateFromPemFile("cert.crt", "private.key")); 
    }));

// Add services to the container.
var allowedOrigings = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services
    .AddCors(options => options
        .AddPolicy("CorsPolicy", policyBuilder => policyBuilder
            .WithOrigins(allowedOrigings)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()))
    .AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseSwagger().UseSwaggerUI();

app
    .UseHttpsRedirection()
    .UseDefaultFiles()
    .UseStaticFiles()
    .UseRouting()
    .UseCors("CorsPolicy")
    .UseAuthorization();

app.MapControllers();
app.MapHub<WebRtcHub>("/signallingHub");
app.MapFallbackToFile("index.html");

app.Run();
