using IdentityManagement;
using Kraken.Host;
using Kraken.Host.Modules;
using KrakenExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddKraken(builder.Configuration ,Bootstrapper.KrakenBuilder());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inyectamos los servicios en la canalizacion de solicitudes
app.UseKraken();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapGet("/", context => context.Response.WriteAsync("Kraken Example API"));
//    endpoints.MapModuleInfo();
//});

app.Run();
