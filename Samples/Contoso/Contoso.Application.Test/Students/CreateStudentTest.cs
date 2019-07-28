using System;
using System.Collections.Generic;
using System.Text;
using NUnit;
using Moq;
using System.Threading.Tasks;
using Contoso.Core.Interfaces.Repositories;
using Contoso.Application.Students.Commands;
using Contoso.Core.Domain;
using System.Threading;
using NUnit.Framework;

namespace Contoso.Application.Test.Students
{
    public class CreateStudentTest
    {
        private readonly Mock<IStudentRepository> studentRepository = new Mock<IStudentRepository>();

        [Test]
        public async Task CreateStudent_ValidStudent_CreateSuccessfully()
        {
            // Arrange
            var student = new CreateStudent("James", "Test", DateTime.Now);

            studentRepository.Setup(s => s.Add(It.IsAny<Student>())).ReturnsAsync(1);
            var handler = new CreateStudentHandler(studentRepository.Object);

            // Act
            var result = await handler.Handle(student, CancellationToken.None);

            // Assert
            result.Match(
                Left: error => Assert.Fail("Validation should have passed"),
                Right: result => Assert.Pass());

        }

        [Test]
        public async Task CreateStudent_InvalidStudent_Fails()
        {
            // Arrange
            var student = new CreateStudent("", "", DateTime.Now);

            studentRepository.Setup(s => s.Add(It.IsAny<Student>())).ReturnsAsync(1);
            var handler = new CreateStudentHandler(studentRepository.Object);

            // Act
            var result = await handler.Handle(student, CancellationToken.None);

            // Assert
            result.Match(
                Left: error => Assert.Pass(),
                Right: result => Assert.Fail("Validation should have failed"));
        }
    }
}
