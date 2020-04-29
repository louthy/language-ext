using System.Collections.Generic;

namespace Contoso.Core.Domain
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int? DepartmentId { get; set; }

        public Department Department { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<CourseAssignment> CourseAssignments { get; set; }
    }
}
