using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Speak.Storage;
using Speak.Telegram.Bot.Entities;
using Speak.Telegram.Bot.FeatureHandlers;
using Speak.Telegram.Bot.FeatureRequests;
using Telegram.Bot;

namespace Speak.Telegram.Bot.Tests;

[NonParallelizable]
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

    [Test]
    public void ShouldCorrectlyFindTodaysPepe()
    {
        // Arrange
        var oldPepe = new TodayPepe("username", "path", DateTimeOffset.Now.Date.AddTicks(-2));
        var newPepe = new TodayPepe("username", "path");
        var secondOldPepe = new TodayPepe("username", "path", DateTimeOffset.Now.Date.AddTicks(-5));
        
        var pepes = new[] { oldPepe, newPepe, secondOldPepe};

        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Func<TodayPepe, bool>>._))
            .ReturnsLazily(cf =>
            {
                var predicate = cf.Arguments[0] as Func<TodayPepe, bool>;
                return Task.FromResult(pepes.FirstOrDefault(predicate));
            });

        // Act
        var todaysPepe = _featureUnderTests.GetCurrentUserAlreadyChosenPepe("username").GetAwaiter().GetResult();
        
        // Assert
        todaysPepe.Should().Be(newPepe);
    }
}