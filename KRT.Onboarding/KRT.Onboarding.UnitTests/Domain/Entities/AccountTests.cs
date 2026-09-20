using FluentAssertions;
using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.Enums;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.UnitTests.Domain.Entities
{
    public class AccountTests
    {
        [Fact]
        public void Construtor_DeveCriarConta_ComDadosInformados()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");

            // Act
            var account = new Account(holderName, cpf);

            // Assert
            account.HolderName.Should().Be(holderName);
            account.Cpf.Should().Be(cpf);
        }

        [Fact]
        public void Construtor_DeveGerarId_QuandoContaForCriada()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");

            // Act
            var account = new Account(holderName, cpf);

            // Assert
            account.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Construtor_DeveCriarContaComoAtiva()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");

            // Act
            var account = new Account(holderName, cpf);

            // Assert
            account.Status.Should().Be(AccountStatus.Active);
        }

        [Fact]
        public void Update_DeveAlterarDadosDaConta()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");
            var account = new Account(holderName, cpf);
            var novoHolderName = new HolderName("Munir Marques Silva");

            // Act
            account.Update(novoHolderName, AccountStatus.Inactive);

            // Assert
            account.HolderName.Should().Be(novoHolderName);
            account.Status.Should().Be(AccountStatus.Inactive);
        }

        [Fact]
        public void Update_DeveLancarDomainException_QuandoStatusForInvalido()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");
            var account = new Account(holderName, cpf);
            var novoHolderName = new HolderName("Munir Marques Silva");
            var statusInvalido = (AccountStatus)999;

            // Act
            Action action = () => account.Update(novoHolderName, statusInvalido);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Invalid account status");
        }
    }
}