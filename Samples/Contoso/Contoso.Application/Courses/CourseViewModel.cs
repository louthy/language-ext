using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;

namespace Contoso.Application.Courses
{
    public class CourseViewModel : Record<CourseViewModel>
    {
        public CourseViewModel(int courseId, string title, int credits, int? departmentId)
        {
            CourseId = courseId;
            Title = title;
            Credits = credits;
            DepartmentId = departmentId;
        }

        public int CourseId { get; }
        public string Title { get; }
        public int Credits { get; }
        public int? DepartmentId { get; }
    }
}
