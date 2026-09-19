using FluentAssertions;
using KRT.Onboarding.Domain.Exceptions;
using KRT.Onboarding.Domain.ValueObjects;

namespace KRT.Onboarding.UnitTests.Domain.ValueObjects
{
    public class HolderNameTests
    {
        [Fact]
        public void Construtor_DeveCriarHolderName_QuandoNomeForValido()
        {
            // Arrange
            var nome = "Munir Marques";

            // Act
            var result = new HolderName(nome);

            // Assert
            result.Value.Should().Be(nome);
        }

        [Fact]
        public void Construtor_DeveNormalizarEspacos_QuandoNomePossuirEspacosExtras()
        {
            // Arrange
            var nome = "  Munir   Marques     ";
            //var nome = "munir   Marques    d ";

            // Act
            var result = new HolderName(nome);

            // Assert
            result.Value.Should().Be("Munir Marques");
        }

        [Fact]
        public void Construtor_DeveAceitarCaracteresAcentuados_QuandoNomeForValido()
        {
            // Arrange
            var nome = "João Gonçalves";

            // Act
            var result = new HolderName(nome);

            // Assert
            result.Value.Should().Be(nome);
        }

        [Fact]
        public void Construtor_DeveAceitarApostrofoEHifen_QuandoNomeForValido()
        {
            // Arrange
            var nome = "Anne-Marie D'Ávila";

            // Act
            var result = new HolderName(nome);

            // Assert
            result.Value.Should().Be(nome);
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomeEstiverVazio()
        {
            // Arrange
            var nome = string.Empty;

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name cannot be empty.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomeTiverMenosDeDoisCaracteres()
        {
            // Arrange
            var nome = "A";

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name must contain at least 2 characters.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomeUltrapassar150Caracteres()
        {
            // Arrange
            var nome = new string('A', 151);

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name cannot exceed 150 characters.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomePossuirNumeros()
        {
            // Arrange
            var nome = "Munir Marques 123";

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name cannot contain numbers.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomeNaoPossuirLetras()
        {
            // Arrange
            var nome = "---";

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name must contain letters.");
        }

        [Fact]
        public void Construtor_DeveLancarDomainException_QuandoNomePossuirCaracteresInvalidos()
        {
            // Arrange
            var nome = "Munir @ Marques";

            // Act
            Action action = () => new HolderName(nome);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("Holder name contains invalid characters.");
        }

        [Fact]
        public void Construtor_DeveCriarHolderName_QuandoNomePossuirExatamente150Caracteres()
        {
            // Arrange
            var nome = new string('A', 150); // cria uma string com exatamente 150 letras A

            // Act
            var result = new HolderName(nome);

            // Assert
            result.Value.Should().HaveLength(150);
        }
    }
}