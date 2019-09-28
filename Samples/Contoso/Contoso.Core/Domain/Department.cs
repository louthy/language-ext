using System;
using System.Collections.Generic;

namespace Contoso.Core.Domain
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public int? AdministratorId { get; set; }
        public Instructor Administrator { get; set; }
        public List<Course> Courses { get; set; }
    }
}
