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
        public void UpdateHolderName_DeveAlterarNomeDaConta()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var novoHolderName = new HolderName("João Gonçalves");
            var cpf = new Cpf("52998224725");
            var account = new Account(holderName, cpf);

            // Act
            account.UpdateHolderName(novoHolderName);

            // Assert
            account.HolderName.Should().Be(novoHolderName);
        }

        [Fact]
        public void ChangeStatus_DeveAlterarStatusDaConta()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");
            var account = new Account(holderName, cpf);

            // Act
            account.ChangeStatus(AccountStatus.Inactive);

            // Assert
            account.Status.Should().Be(AccountStatus.Inactive);
        }

        [Fact]
        public void ChangeStatus_DeveLancarDomainException_QuandoStatusForInvalido()
        {
            // Arrange
            var holderName = new HolderName("Munir Marques");
            var cpf = new Cpf("52998224725");
            var account = new Account(holderName, cpf);
            var statusInvalido = (AccountStatus)999; // Força um valor inexistente no enum

            // Act
            Action action = () => account.ChangeStatus(statusInvalido);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Invalid account status");
        }
    }
}