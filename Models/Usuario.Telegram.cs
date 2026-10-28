namespace Lumina_Reserve_MVC.Models;

public partial class Usuario
{
    // Chat de Telegram del usuario (se guarda cuando le escribe al bot)
    public long? TelegramChatId { get; set; }
}