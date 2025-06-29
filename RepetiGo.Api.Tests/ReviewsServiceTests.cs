using AutoMapper;

using FluentAssertions;

using Moq;

using RepetiGo.Api.Enums;
using RepetiGo.Api.Interfaces.Repositories;
using RepetiGo.Api.MappingProfiles;
using RepetiGo.Api.Models;
using RepetiGo.Api.Services;

namespace RepetiGo.Api.Tests
{
    public class ReviewsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICardsRepository> _mockCardsRepository;
        private readonly IMapper _mapper;
        private readonly ReviewsService _reviewService;
        private readonly Settings _defaultSettings;

        public ReviewsServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCardsRepository = new Mock<ICardsRepository>();
            _mockUnitOfWork.Setup(uow => uow.CardsRepository).Returns(_mockCardsRepository.Object);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReviewMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _reviewService = new ReviewsService(_mockUnitOfWork.Object, _mapper);

            _defaultSettings = new Settings
            {
                LearningSteps = "25m 1d",
                RelearningSteps = "30m",
                GraduatingInterval = 3,
                EasyInterval = 4,
                MinimumInterval = 1,
                MaximumInterval = 180,
                EasyBonus = 1.5,
                HardInterval = 1.2,
                NewInterval = 0.2
            };
        }

        [Fact]
        public async Task ProcessReview_NewCard_Again_ShouldResetToFirstLearningStep()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.New,
                LearningStep = 0,
                NextReview = DateTime.UtcNow
            };
            // Tell the mock repository what to do when UpdateAsync is called
            // It.IsAny<Card>() means "accept any Card object"
            // Returns(Task.CompletedTask) means "return a completed task"
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            // Tell the mock repository what to do when GetByIdAsync is called
            // ReturnsAsync(1) means "return a task that completes with the value 1"
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Again, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(25), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(0);
            card.Status.Should().Be(CardStatus.Learning);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(0);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_NewCard_Hard_ShouldAdvanceToNextLearningStep()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.New,
                LearningStep = 0,
                NextReview = DateTime.UtcNow
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Hard, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(0);
            card.Status.Should().Be(CardStatus.Learning);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_NewCard_Good_ShouldAdvanceToNextLearningStep()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.New,
                LearningStep = 0,
                NextReview = DateTime.UtcNow
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Good, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(0);
            card.Status.Should().Be(CardStatus.Learning);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_NewCard_Easy_ShouldGraduateToReview()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.New,
                LearningStep = 0,
                NextReview = DateTime.UtcNow
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Easy, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(4), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(1);
            card.Status.Should().Be(CardStatus.Review);
            card.EasinessFactor.Should().Be(2.65);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_LearningCard_Again_ShouldResetToFirstStep()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Learning,
                LearningStep = 1,
                NextReview = DateTime.UtcNow,
                Repetition = 0,
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Again, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(25), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(0);
            card.Status.Should().Be(CardStatus.Learning);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(0);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_LearningCard_Good_LastStep_ShouldGraduateToReview()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Learning,
                LearningStep = 1,
                NextReview = DateTime.UtcNow,
                Repetition = 0,
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Good, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(3), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(1);
            card.Status.Should().Be(CardStatus.Review);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_ReviewCard_Again_ShouldResetToRelearning()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Review,
                NextReview = DateTime.UtcNow,
                Repetition = 5,
                LastReviewed = DateTime.UtcNow.AddDays(-10),
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Again, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(0);
            card.Status.Should().Be(CardStatus.Relearning);
            card.EasinessFactor.Should().BeApproximately(2.18, 0.01);
            card.LearningStep.Should().Be(0);
            card.FailedInterval.Should().BeApproximately(10 * _defaultSettings.NewInterval, 0.01);
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_ReviewCard_Good_FirstReview_ShouldUseGraduatingInterval()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Review,
                NextReview = DateTime.UtcNow,
                Repetition = 0,
                LastReviewed = DateTime.UtcNow.AddDays(-10),
                LearningStep = 1,
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Good, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(3), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(1);
            card.Status.Should().Be(CardStatus.Review);
            card.EasinessFactor.Should().Be(2.5);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_ReviewCard_Good_SecondReview_ShouldUseEasinessFactor()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Review,
                NextReview = DateTime.UtcNow,
                Repetition = 1,
                LastReviewed = DateTime.UtcNow.AddDays(-10),
                LearningStep = 1,
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Good, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(7.5), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(2);
            card.Status.Should().Be(CardStatus.Review);
            card.EasinessFactor.Should().BeApproximately(2.5, 0.01);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessReview_ReviewCard_Hard_ShouldApplyHardInterval()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Status = CardStatus.Review,
                NextReview = DateTime.UtcNow,
                Repetition = 1,
                LastReviewed = DateTime.UtcNow.AddDays(-10),
                LearningStep = 1,
                EasinessFactor = 2.5
            };
            _mockCardsRepository.Setup(c => c.UpdateAsync(It.IsAny<Card>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).ReturnsAsync(1);

            // Act
            await _reviewService.ProcessReview(card, ReviewRating.Hard, _defaultSettings);

            // Assert
            card.NextReview.Should().BeCloseTo(DateTime.UtcNow.AddDays(12), TimeSpan.FromSeconds(1));
            card.Repetition.Should().Be(2);
            card.Status.Should().Be(CardStatus.Review);
            card.EasinessFactor.Should().BeApproximately(2.36, 0.01);
            card.LearningStep.Should().Be(1);
            card.FailedInterval.Should().BeNull();
            card.LastReviewed.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// ProcessReview_ReviewCard_Easy_ShouldApplyEasyBonus
        /// ProcessReview_RelearningCard_Good_ShouldUseFailedInterval
        /// ProcessReview_RelearningCard_Easy_ShouldUseEasyInterval
        /// ProcessReview_EmptyLearningSteps_ShouldUseMinimumInterval
        /// ProcessReview_EmptyRelearningSteps_ShouldUseMinimumInterval
        /// ProcessReview_IntervalClamping_ShouldRespectMinimumAndMaximum
        /// ProcessReview_EasinessFactorUpdate_ShouldFollowSM2Algorithm
        /// ProcessReview_EasinessFactorUpdate_EasyRating_ShouldIncrease
        /// ProcessReview_EasinessFactorUpdate_HardRating_ShouldDecrease
        /// ProcessReview_EasinessFactorUpdate_AgainRating_ShouldDecreaseSignificantly
        /// ProcessReview_EasinessFactorMinimum_ShouldNotGoBelow1_3
        /// ProcessPreviewReviews_ShouldReturnCorrectReviewResponses
        /// ParseSteps_ShouldHandleValidFormats
        /// ParseSteps_ShouldThrowOnInvalidFormat
        /// </summary>
    }
}
