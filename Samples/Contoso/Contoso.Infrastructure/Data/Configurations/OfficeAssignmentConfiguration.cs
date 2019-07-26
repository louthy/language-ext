using Contoso.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.Infrastructure.Data.Configurations
{
    public class OfficeAssignmentConfiguration : IEntityTypeConfiguration<OfficeAssignment>
    {
        public void Configure(EntityTypeBuilder<OfficeAssignment> builder)
        {
            builder.Property(b => b.Location)
                .HasMaxLength(50);
        }
    }
}
