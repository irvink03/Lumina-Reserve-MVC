using System.Globalization;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Lumina_Reserve_MVC.Models;

namespace Lumina_Reserve_MVC.Services;

public class TelegramNotificador
{
    private readonly TelegramBotClient _bot;
    private readonly ILogger<TelegramNotificador> _logger;

    public TelegramNotificador(IConfiguration config, ILogger<TelegramNotificador> logger)
    {
        _logger = logger;

        var token = config["TelegramBot:Token"]?.Trim();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Falta TelegramBot:Token en la configuración");

        _bot = new TelegramBotClient(token);
    }

    // Envía un mensaje; devuelve true si Telegram lo aceptó
    public async Task<bool> EnviarAsync(long chatId, string mensajeHtml)
    {
        try
        {
            await _bot.SendMessage(chatId, mensajeHtml, parseMode: ParseMode.Html);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("No se pudo enviar el aviso por Telegram: {Message}", ex.Message);
            return false;
        }
    }

    // Arma el mensaje de "cita agendada"
    public static string ConstruirMensajeCita(Cita cita)
    {
        var es = new CultureInfo("es-MX");

        string cliente = WebUtility.HtmlEncode(cita.Cliente.NombreCompleto);
        string sucursal = WebUtility.HtmlEncode(cita.Sucursal.Nombre);
        string direccion = WebUtility.HtmlEncode(cita.Sucursal.Direccion);
        string barbero = WebUtility.HtmlEncode(cita.Barbero.Usuario.NombreCompleto);
        string servicios = WebUtility.HtmlEncode(
            string.Join(", ", cita.CitaCortes.Select(cc => cc.Corte.Nombre)));

        decimal total = cita.Pago != null
            ? cita.Pago.MontoTotal
            : cita.CitaCortes.Sum(cc => cc.PrecioCobrado);

        string fecha = cita.FechaCita.ToString("dddd d 'de' MMMM 'de' yyyy", es);
        string hora = cita.HoraCita.ToString("HH:mm");

        // Llegar 2 horas antes (sin pasar de la hora de apertura de la sucursal)
        var llegada = cita.HoraCita.AddMinutes(-120);
        var apertura = cita.Sucursal.HoraApertura;
        string textoLlegada;

        if (apertura != null && llegada < apertura.Value)
        {
            textoLlegada = $"Tu cita es temprano: llega a partir de las {apertura.Value:HH:mm}, cuando abre la sucursal.";
        }
        else
        {
            textoLlegada = $"<b>Debes llegar 2 horas antes</b>, a las {llegada:HH:mm} hrs.";
        }

        return
            "✅ <b>¡Tu cita ha sido agendada!</b>\n\n" +
            $"Hola, <b>{cliente}</b>. Estos son los detalles:\n\n" +
            $"📅 <b>Fecha:</b> {fecha}\n" +
            $"🕒 <b>Hora:</b> {hora} hrs\n" +
            $"📍 <b>Sucursal:</b> {sucursal}\n" +
            $"🗺️ {direccion}\n" +
            $"💈 <b>Barbero:</b> {barbero}\n" +
            $"✂️ <b>Servicios:</b> {servicios}\n" +
            $"💰 <b>Total:</b> ${total:N2}\n\n" +
            $"⏰ {textoLlegada}\n\n" +
            "¡Te esperamos en Lumina Reserve! 💈";
    }
}