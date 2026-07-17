using SistemaBancario.API.Interfaces;
using SistemaBancario.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new CuentaRepository(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()  // Permite peticiones de cualquier puerto (Blazor, Angular, React)
              .AllowAnyHeader()  // Permite cualquier tipo de cabecera (incluyendo tokens)
              .AllowAnyMethod(); // Permite GET, POST, PUT, DELETE
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // Asegúrate de que esta línea exista

// 2. ACTIVAR CORS (Debe ir exactamente aquí, antes de la autorización)
app.UseCors("PermitirFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
