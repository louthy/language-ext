using Contoso.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.Infrastructure.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(b => b.Title)
                .HasMaxLength(50);
        }
    }
}
