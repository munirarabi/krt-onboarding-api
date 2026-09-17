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
            builder.ToTable("Accounts");

            // pk da tabela
            builder.HasKey(x => x.Id);

            // significa que o GUID é gerado no proprio dominio
            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            // obrigatorio (not null) e máx de 150 chars.
            builder.Property(x => x.HolderName)
                .IsRequired()
                .HasMaxLength(150);

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
        }
    }
}