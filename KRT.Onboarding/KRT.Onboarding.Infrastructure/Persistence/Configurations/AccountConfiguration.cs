using KRT.Onboarding.Domain.Entities;
using KRT.Onboarding.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KRT.Onboarding.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // tabela
            // Permitindo somente registros com 0 ou 1 porque são os dois status existentes atualmente.
            builder.ToTable("Accounts", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Accounts_Status",
                    "[Status] IN (0, 1)");
            });

            // pk da tabela
            builder.HasKey(x => x.Id);

            // significa que o GUID é gerado no proprio dominio
            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            // obrigatorio (not null) e máx de 150 chars.
            builder.Property(x => x.HolderName)
                /*
                 isso ensina o EF como converter o Value Object HolderName para um tipo que o SQL Server consegue armazenar e vice-versa....
                 */
                .HasConversion(
                    holderName => holderName.Value,
                    value => new HolderName(value))
                .HasMaxLength(150)
                .IsRequired();

            // diz pro EF core que precisa tratar o cpf como value (string), max de 11 e é NOT null
            builder.Property(x => x.Cpf)
                .HasConversion(
                    cpf => cpf.Value,
                    value => new Cpf(value))
                .HasMaxLength(11)
                .IsRequired();

            builder.HasIndex(x => x.Cpf)
                .IsUnique();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);
        }
    }
}