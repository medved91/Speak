using System;
using System.IO;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Speak.Storage;
using Speak.TelegramBot.Entities;
using Speak.TelegramBot.FeatureHandlers;
using Speak.TelegramBot.FeatureRequests;
using Telegram.Bot;

namespace Speak.TelegramBot.Tests;

public class PickWhichPepeAmITodayFeatureHandlerTests
{
    private PickWhichPepeAmITodayHandler _featureUnderTests = null!;
    private ITelegramBotClient _fakeBotClient = null!;
    private IRepository<TodayPepe> _fakeRepo = null!;

    [SetUp]
    public void Setup()
    {
        _fakeBotClient = A.Fake<ITelegramBotClient>();
        _fakeRepo = A.Fake<IRepository<TodayPepe>>();
        _featureUnderTests = new PickWhichPepeAmITodayHandler(_fakeBotClient, _fakeRepo);
    }

    [Test]
    public void ShouldAddPepeOnce_WhenHasAlreadyPickedPepe()
    {
        // Arrange
        var request = new PickWhichPepeAmITodayRequest("testUserName", 1, 1);

        var firstPepeImagePath = Directory.GetFiles("Files").First();
        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Func<TodayPepe, bool>>._))
            .Returns((TodayPepe?)null).Once()
            .Then
            .Returns(new TodayPepe("testUsername", firstPepeImagePath));

        // Act
        _featureUnderTests.Handle(request).GetAwaiter().GetResult();
        _featureUnderTests.Handle(request).GetAwaiter().GetResult();

        // Assert
        A.CallTo(() => _fakeRepo.AddAsync(A<TodayPepe>.That.Matches(p => p.Username == request.Username)))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Func<TodayPepe,bool>>._))
            .MustHaveHappenedTwiceExactly();
    }
}