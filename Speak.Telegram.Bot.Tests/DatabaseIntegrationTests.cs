using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Speak.Telegram.Postgres;

namespace Speak.Telegram.Bot.Tests;

#nullable disable

[TestFixture, Explicit]
public class DatabaseIntegrationTests
{
    private TelegramBotDbContext _context;

    [OneTimeSetUp]
    public void SetUp()
    {
        var dbContextOptions = new DbContextOptionsBuilder<TelegramBotDbContext>()
            .UseNpgsql("Host=face-2-face.ru;Port=5432;Database=postgres;Username=secret;Password=secret;Pooling=true;Maximum Pool Size=100")
            .LogTo(Console.WriteLine)
            .Options;
        
        _context = new TelegramBotDbContext(dbContextOptions);
    }
    
    [Test]
    public void ShouldEditEntityWithShadowPrimaryKey()
    {
        var chat = _context.Chats.First();

        chat.TelegramChatId = 5;

        _context.SaveChanges();
    }
}