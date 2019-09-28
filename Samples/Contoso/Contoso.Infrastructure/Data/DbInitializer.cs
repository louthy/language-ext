using System;
using System.Linq;
using Contoso.Core.Domain;
using LanguageExt;

namespace Contoso.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static Unit Initialize(ContosoDbContext context)
        {
            // Look for any students.
            if (context.Students.Any())
            {
                return Unit.Default;   // DB has been seeded
            }

            var students = new Student[]
            {
                new Student { FirstName = "Carson",   LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Student { FirstName = "Meredith", LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstName = "Arturo",   LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstName = "Gytis",    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstName = "Yan",      LastName = "Li",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstName = "Peggy",    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Student { FirstName = "Laura",    LastName = "Norman",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstName = "Nino",     LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01") }
            };

            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var instructors = new Instructor[]
            {
                new Instructor { FirstName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12") }
            };

            foreach (Instructor i in instructors)
            {
                context.Instructors.Add(i);
            }
            context.SaveChanges();

            var departments = new Department[]
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    AdministratorId  = instructors.Single( i => i.LastName == "Abercrombie").InstructorId },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    AdministratorId  = instructors.Single( i => i.LastName == "Fakhouri").InstructorId },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    AdministratorId  = instructors.Single( i => i.LastName == "Harui").InstructorId },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    AdministratorId  = instructors.Single( i => i.LastName == "Kapoor").InstructorId }
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course { Title = "Chemistry",      Credits = 3,
                    DepartmentId = departments.Single( s => s.Name == "Engineering").DepartmentId
                },
                new Course { Title = "Microeconomics", Credits = 3,
                    DepartmentId = departments.Single( s => s.Name == "Economics").DepartmentId
                },
                new Course { Title = "Macroeconomics", Credits = 3,
                    DepartmentId = departments.Single( s => s.Name == "Economics").DepartmentId
                },
                new Course { Title = "Calculus",       Credits = 4,
                    DepartmentId = departments.Single( s => s.Name == "Mathematics").DepartmentId
                },
                new Course { Title = "Trigonometry",   Credits = 4,
                    DepartmentId = departments.Single( s => s.Name == "Mathematics").DepartmentId
                },
                new Course { Title = "Composition",    Credits = 3,
                    DepartmentId = departments.Single( s => s.Name == "English").DepartmentId
                },
                new Course { Title = "Literature",     Credits = 4,
                    DepartmentId = departments.Single( s => s.Name == "English").DepartmentId
                },
            };

            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment {
                    InstructorId = instructors.Single( i => i.LastName == "Fakhouri").InstructorId,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    InstructorId = instructors.Single( i => i.LastName == "Harui").InstructorId,
                    Location = "Gowan 27" },
                new OfficeAssignment {
                    InstructorId = instructors.Single( i => i.LastName == "Kapoor").InstructorId,
                    Location = "Thompson 304" },
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }
            context.SaveChanges();

            var courseInstructors = new CourseAssignment[]
            {
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Kapoor").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Calculus" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Fakhouri").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Composition" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").InstructorId
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Literature" ).DepartmentId.Value,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").InstructorId
                    },
            };

            foreach (CourseAssignment ci in courseInstructors)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alexander").StudentId,
                    CourseId = courses.Single(c => c.Title == "Chemistry" ).CourseId,
                    Grade = Grade.A
                },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alexander").StudentId,
                    CourseId = courses.Single(c => c.Title == "Microeconomics" ).CourseId,
                    Grade = Grade.C
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alexander").StudentId,
                    CourseId = courses.Single(c => c.Title == "Macroeconomics" ).CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alonso").StudentId,
                    CourseId = courses.Single(c => c.Title == "Calculus" ).CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alonso").StudentId,
                    CourseId = courses.Single(c => c.Title == "Trigonometry" ).CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Alonso").StudentId,
                    CourseId = courses.Single(c => c.Title == "Composition" ).CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Anand").StudentId,
                    CourseId = courses.Single(c => c.Title == "Chemistry" ).CourseId
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Anand").StudentId,
                    CourseId = courses.Single(c => c.Title == "Microeconomics").CourseId,
                    Grade = Grade.B
                    },
                new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Barzdukas").StudentId,
                    CourseId = courses.Single(c => c.Title == "Chemistry").CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Li").StudentId,
                    CourseId = courses.Single(c => c.Title == "Composition").CourseId,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    StudentId = students.Single(s => s.LastName == "Justice").StudentId,
                    CourseId = courses.Single(c => c.Title == "Literature").CourseId,
                    Grade = Grade.B
                    }
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                            s.Student.StudentId == e.StudentId &&
                            s.Course.CourseId == e.CourseId).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
            return Unit.Default;
        }
    }
}
