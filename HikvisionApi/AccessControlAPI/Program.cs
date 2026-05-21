using AMPMAccesControlAPI.DI;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var host = configuration["host"];
var port = configuration["port"];

if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
{
	throw new Exception("host and port must be set in appsettings.json");
}

var url = $"{host}:{port}";

// Add services to the container.

builder.Services.AddControllers().AddXmlSerializerFormatters();
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

// 1️⃣ Agregar el servicio CORS
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

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Configurar la aplicación para usar CORS
app.UseCors("AllowFrontend");

app.MapControllers();


app.Run();

