using FluentAssertions;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.UnitTests.Domain.ValueObjects
{
    public class CpfTests
    {
        [Fact]
        public void Construtor_DeveCriarCpf_QuandoCpfForValido()
        {
            // Arrange
            var cpf = "52998224725"; // CPF valido
            //var cpf = "02998224725"; // CPF invalido

            // Act
            var result = new Cpf(cpf);

            // Assert
            result.Value.Should().Be(cpf);
        }

        [Fact]
        public void Construtor_DeveNormalizarCpf_QuandoCpfEstiverFormatado()
        {
            // Arrange
            var cpf = "529.982.247-25";

            // Act
            var result = new Cpf(cpf);

            // Assert
            result.Value.Should().Be("52998224725");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoCpfForInvalido()
        {
            // Arrange
            var cpf = "12345678900";

            // Act
            Action action = () => new Cpf(cpf);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Invalid CPF.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoCpfEstiverVazio()
        {
            // Arrange
            var cpf = string.Empty;

            // Act
            Action action = () => new Cpf(cpf);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("CPF cannot be empty.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoCpfTiverDigitosRepetidos()
        {
            // Arrange
            var cpf = "11111111111";

            // Act
            Action action = () => new Cpf(cpf);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Invalid CPF.");
        }
    }
}