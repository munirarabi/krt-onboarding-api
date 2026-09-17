using KRT.Onboarding.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KRT.Onboarding.Infrastructure.Persistence.Context
{
    public class OnboardingDbContext : DbContext
    {
        public OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnboardingDbContext).Assembly);
        }
    }
}