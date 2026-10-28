using Microsoft.EntityFrameworkCore;
using Lumina_Reserve_MVC.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios MVC al contenedor.
builder.Services.AddControllersWithViews();

// 2. Agregar la conexión a la base de datos SQL Server mediante Entity Framework
builder.Services.AddDbContext<LuminaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LuminaConnection")));

// 3. Configurar el servicio de Sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Bot de Telegram: notificador (avisos de citas) y servicio que escucha mensajes
builder.Services.AddSingleton<Lumina_Reserve_MVC.Services.TelegramNotificador>();
builder.Services.AddHostedService<Lumina_Reserve_MVC.Services.TelegramBotService>();

var app = builder.Build();

// 5. Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();