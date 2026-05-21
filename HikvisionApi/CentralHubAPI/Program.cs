using Microsoft.AspNetCore.Authentication.JwtBearer;
using AMPM_CentralHubAPI.DI;
using AMPM_CentralHubAPI.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var host = configuration["host"];
var port = configuration["port"];

if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
{
    throw new Exception("host and port must be set in appsettings.json");
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddApplication(configuration);
builder.Services.AddInfrastructure(configuration);

// 1?? Agregar el servicio CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .WithHeaders([
                "Content-Type",
                "Authorization",
                "X-Requested-With",
                "Accept"
            ])
            .WithMethods(["GET", "POST", "PUT", "DELETE"]);
    });
});

builder.Services.AddSingleton<JwtCripto>();
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => { 
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Configurar la aplicación para usar CORS
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
