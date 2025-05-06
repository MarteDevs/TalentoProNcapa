using TalentoPro.Application;
using TalentoPro.Infrastructure;
using TalentoPro.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar servicios de dominio
builder.Services.AddScoped<IEmpleadoService, ServicioEmpleado>();
builder.Services.AddScoped<IDepartamentoService, ServicioDepartamento>();
builder.Services.AddScoped<ICapacitacionService, ServicioCapacitacion>();
builder.Services.AddScoped<IInscripcionService, ServicioInscripcion>();

// Registrar servicios de infraestructura
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registrar servicios de aplicación
builder.Services.AddApplicationServices();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
