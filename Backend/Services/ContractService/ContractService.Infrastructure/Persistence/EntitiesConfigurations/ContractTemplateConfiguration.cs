using ContractService.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Infrastructure.Persistence.EntitiesConfigurations;

public class ContractTemplateConfiguration : IEntityTypeConfiguration<ContractTemplate>
{
    public void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        builder.ToTable("ContractTemplates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}