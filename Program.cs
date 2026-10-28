using Microsoft.EntityFrameworkCore;
using Lumina_Reserve_MVC.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios MVC al contenedor.
builder.Services.AddControllersWithViews();

// 2. NUEVO: Agregar la conexión a la base de datos SQL Server mediante Entity Framework
builder.Services.AddDbContext<LuminaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LuminaConnection")));

var app = builder.Build();

// 3. Configurar el pipeline de solicitudes HTTP (Middlewares).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor HSTS por defecto es de 30 días. Se recomienda cambiarlo en producción.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. Mapeo de las rutas por defecto (Patrón MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();