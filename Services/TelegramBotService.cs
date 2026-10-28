using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Lumina_Reserve_MVC.Models;

namespace Lumina_Reserve_MVC.Services;

public class TelegramBotService : BackgroundService
{
    private readonly TelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(IConfiguration config, IServiceScopeFactory scopeFactory, ILogger<TelegramBotService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var token = config["TelegramBot:Token"]?.Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Falta TelegramBot:Token en appsettings.json");

        _bot = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var me = await _bot.GetMe(ct);
            _logger.LogInformation("Bot conectado: @{Username}", me.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError("No se pudo conectar el bot (revisa el token): {Message}", ex.Message);
            return;
        }

        _bot.StartReceiving(
            OnUpdate,
            OnError,
            new ReceiverOptions { AllowedUpdates = [UpdateType.Message] },
            ct);

        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task OnUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message?.Text is not { } text) return;

        var chatId = update.Message.Chat.Id;
        text = text.Trim();

        if (text.StartsWith("/start"))
        {
            await bot.SendMessage(chatId,
                "¡Hola! Bienvenido a Lumina Reserve 💈\n\nEnvíame el correo con el que te registraste en la página y te daré tu código de acceso. Aquí también recibirás los avisos de tus citas.",
                cancellationToken: ct);
            return;
        }

        if (!MailAddress.TryCreate(text, out _))
        {
            await bot.SendMessage(chatId, "Por favor, envíame un correo electrónico válido.", cancellationToken: ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LuminaDbContext>();

        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == text, ct);
        if (usuario is null)
        {
            await bot.SendMessage(chatId,
                "⚠️ No encontramos una cuenta con ese correo. Regístrate primero en la página web.",
                cancellationToken: ct);
            return;
        }

        // Guardamos el chat del usuario para poder avisarle de sus citas
        usuario.TelegramChatId = chatId;

        var codigo = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        db.CodigosVerificacionBots.Add(new CodigosVerificacionBot
        {
            UsuarioId = usuario.Id,
            Codigo = codigo,
            FechaGeneracion = DateTime.Now,
            FechaExpiracion = DateTime.Now.AddMinutes(5),
            Usado = false
        });
        await db.SaveChangesAsync(ct);

        await bot.SendMessage(chatId,
            $"🔑 Tu código de verificación es: *{codigo}*\n\n_Válido por 5 minutos._\nRegresa a la página e ingrésalo para continuar.\n\n🔔 Desde ahora recibirás aquí los avisos de tus citas.",
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    private Task OnError(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        _logger.LogError("Error de Telegram: {Message}", ex.Message);
        return Task.CompletedTask;
    }
}