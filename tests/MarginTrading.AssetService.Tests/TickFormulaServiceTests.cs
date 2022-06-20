using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Correlation;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public class TickFormulaServiceTests
    {
        private const string Username = "username";
        private const string Id = "id";

        private readonly Mock<ITickFormulaRepository> _tickFormulaRepoMock = new Mock<ITickFormulaRepository>();
        private readonly Mock<IAuditService> _auditServiceMock = new Mock<IAuditService>();
        private readonly Mock<ICqrsEntityChangedSender> _cqrsSenderMock = new Mock<ICqrsEntityChangedSender>();
        private readonly Mock<CorrelationContextAccessor> _correlationContextAccessor = new Mock<CorrelationContextAccessor>();
        private readonly Mock<IIdentityGenerator> _identityGenerator = new Mock<IIdentityGenerator>();

        [Fact]
        public async Task AddTickFormula_LaddersAndTicksWithDifferentLengths_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0, 1 },
                PdlTicks = new List<decimal> { 1 }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersAndTicksMustHaveEqualLengths, actual.Error);
        }

        [Fact]
        public async Task AddTickFormula_LaddersWithNegativeValue_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0, -1 },
                PdlTicks = new List<decimal> { 0.5M, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersValuesMustBeGreaterOrEqualToZero, actual.Error);
        }

        [Fact]
        public async Task AddTickFormula_TicksWithNonPositiveValue_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0, 1 },
                PdlTicks = new List<decimal> { 0, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlTicksValuesMustBeGreaterThanZero, actual.Error);
        }

        [Fact]
        public async Task AddTickFormula_LaddersDoNotStartFrom0_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0.5M, 1 },
                PdlTicks = new List<decimal> { 0.1M, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersMustStartFromZero, actual.Error);
        }

        [Fact]
        public async Task AddTickFormula_LaddersAreNotAscendingSortedWithoutDuplicates_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0, 1, 1, 2 },
                PdlTicks = new List<decimal> { 0.1M, 0.55M, 0.6M, 0.8M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersMustBeInAscendingOrderWithoutDuplicates, actual.Error);
        }

        [Fact]
        public async Task AddTickFormula_TicksAreNotAscendingSorted_ErrorReturned()
        {
            var request = new TickFormula
            {
                PdlLadders = new List<decimal> { 0, 1, 2 },
                PdlTicks = new List<decimal> { 0.5M, 0.1M, 0.7M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.AddAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlTicksMustBeInAscendingOrder, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_TickFormulaDoesNotExist_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync((ITickFormula) null);

            var request = new TickFormula
            {
                Id = Id,
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.TickFormulaDoesNotExist, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_LaddersAndTicksWithDifferentLengths_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula{Id = Id});

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0, 1 },
                PdlTicks = new List<decimal> { 1 }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersAndTicksMustHaveEqualLengths, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_LaddersWithNegativeValue_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula { Id = Id });

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0, -1 },
                PdlTicks = new List<decimal> { 0.5M, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersValuesMustBeGreaterOrEqualToZero, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_TicksWithNonPositiveValue_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula { Id = Id });

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0, 1 },
                PdlTicks = new List<decimal> { 0, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlTicksValuesMustBeGreaterThanZero, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_LaddersDoNotStartFrom0_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula { Id = Id });

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0.5M, 1 },
                PdlTicks = new List<decimal> { 0.1M, 0.55M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersMustStartFromZero, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_LaddersAreNotAscendingSortedWithoutDuplicates_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula { Id = Id });

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0, 1, 1, 2 },
                PdlTicks = new List<decimal> { 0.1M, 0.55M, 0.6M, 0.8M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlLaddersMustBeInAscendingOrderWithoutDuplicates, actual.Error);
        }

        [Fact]
        public async Task UpdateTickFormula_TicksAreNotAscendingSorted_ErrorReturned()
        {
            _tickFormulaRepoMock.Setup(x => x.GetByIdAsync(Id))
                .ReturnsAsync(new TickFormula { Id = Id });

            var request = new TickFormula
            {
                Id = Id,
                PdlLadders = new List<decimal> { 0, 1, 2 },
                PdlTicks = new List<decimal> { 0.5M, 0.1M, 0.7M }
            };

            var sut = CreateSutInstance();

            var actual = await sut.UpdateAsync(request, Username);

            Assert.Equal(TickFormulaErrorCodes.PdlTicksMustBeInAscendingOrder, actual.Error);
        }

        private TickFormulaService CreateSutInstance()
        {
            return new TickFormulaService(
                _tickFormulaRepoMock.Object,
                _auditServiceMock.Object,
                _cqrsSenderMock.Object);
        }
    }
}