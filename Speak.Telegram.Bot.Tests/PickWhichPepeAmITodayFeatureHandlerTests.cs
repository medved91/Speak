using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.PepeFeature;
using Speak.Telegram.PepeFeature.Entities;
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
        var request = new PickWhichPepeAmITodayFeatureRequest("testUserName", 1, 1);

        var firstPepeImagePath = Directory.GetFiles("Files").First();
        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Expression<Func<TodayPepe, bool>>>._, A<CancellationToken>._))
            .Returns((TodayPepe?)null).Once()
            .Then
            .Returns(new TodayPepe("testUsername", firstPepeImagePath));

        // Act
        _featureUnderTests.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
        _featureUnderTests.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

        // Assert
        A.CallTo(() => _fakeRepo.AddAsync(A<TodayPepe>.That.Matches(p => p.Username == request.Username), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Expression<Func<TodayPepe,bool>>>._, A<CancellationToken>._))
            .MustHaveHappenedTwiceExactly();
    }

    [Test]
    public void ShouldCorrectlyFindTodaysPepe()
    {
        // Arrange
        var oldPepe = new TodayPepe("username", "path", DateTimeOffset.Now.Date.AddTicks(-2));
        var newPepe = new TodayPepe("username", "path");
        var secondOldPepe = new TodayPepe("username", "path", DateTimeOffset.Now.Date.AddTicks(-5));

        var pepes = new[] { oldPepe, newPepe, secondOldPepe };

        A.CallTo(() => _fakeRepo.FirstOrDefaultAsync(A<Expression<Func<TodayPepe, bool>>>._, A<CancellationToken>._))
            .ReturnsLazily(cf =>
            {
                var predicate = cf.Arguments[0] as Expression<Func<TodayPepe, bool>>;
                return Task.FromResult(pepes.FirstOrDefault(predicate!.Compile()));
            });

        // Act
        var todaysPepe = _featureUnderTests
            .GetCurrentUserAlreadyChosenPepe("username", CancellationToken.None).GetAwaiter().GetResult();

        // Assert
        todaysPepe.Should().Be(newPepe);
    }
}