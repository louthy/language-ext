using System;
using System.Collections.Generic;

namespace Contoso.Core.Domain
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => LastName + ", " + FirstName;
        public DateTime HireDate { get; set; }
        public List<CourseAssignment> CourseAssignments { get; set; }
    }
}
