using System;
using System.Collections.Generic;
using System.Text;
using Contoso.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.Infrastructure.Data.Configurations
{
    class CourseAssignmentConfiguration : IEntityTypeConfiguration<CourseAssignment>
    {
        public void Configure(EntityTypeBuilder<CourseAssignment> builder)
        {
            builder.HasOne(c => c.Course)
                .WithMany(c => c.CourseAssignments)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.CourseAssignments)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
