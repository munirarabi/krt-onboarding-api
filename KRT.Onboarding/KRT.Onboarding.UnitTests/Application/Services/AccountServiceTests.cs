using FluentAssertions;
using KRT.Onboarding.Application.DTOs;
using KRT.Onboarding.Application.Interfaces.Caching;
using KRT.Onboarding.Application.Interfaces.Messaging;
using KRT.Onboarding.Application.Interfaces.Repositories;
using KRT.Onboarding.Application.Services;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace KRT.Onboarding.UnitTests.Application.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IAccountCacheService> _accountCacheServiceMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountCacheServiceMock = new Mock<IAccountCacheService>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _loggerMock = new Mock<ILogger<AccountService>>();

            _accountService = new AccountService(
                _accountRepositoryMock.Object,
                _accountCacheServiceMock.Object,
                _eventPublisherMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarConta_QuandoCpfNaoExistir()
        {
            // Arrange
            var holderName = "Munir Marques";
            var cpf = "52998224725";

            //Quando o método ExistsByCpfAsync do repositório for chamado
            //com esse CPF, retorne false,
            //independentemente do CancellationToken utilizado.
            _accountRepositoryMock
                .Setup(x => x.ExistsByCpfAsync(
                    cpf,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _accountService.CreateAsync(
                holderName,
                cpf,
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.HolderName.Should().Be(holderName);
            result.Cpf.Should().Be(cpf);
            result.Status.Should().Be(AccountStatus.Active);

            _accountRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Account>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarConflictException_QuandoCpfJaExistir()
        {
            // Arrange
            var holderName = "Munir Marques";
            var cpf = "52998224725";

            _accountRepositoryMock
                .Setup(x => x.ExistsByCpfAsync(cpf, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> action = () => _accountService.CreateAsync(
                holderName,
                cpf,
                CancellationToken.None);

            // Assert
            await action.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("An account with this CPF already exists.");
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarContaDoCache_QuandoContaEstiverEmCache()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var accountDto = new AccountDto
            {
                Id = accountId,
                HolderName = "Munir Marques",
                Cpf = "52998224725",
                Status = AccountStatus.Active
            };

            _accountCacheServiceMock
                .Setup(x => x.GetAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDto);

            // Act
            var result = await _accountService.GetByIdAsync(
                accountId,
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(accountId);
            result.HolderName.Should().Be("Munir Marques");
            result.Cpf.Should().Be("52998224725");
            result.Status.Should().Be(AccountStatus.Active);
        }
    }
}