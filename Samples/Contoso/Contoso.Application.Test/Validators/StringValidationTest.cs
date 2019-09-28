using System.Linq;
using Contoso.Core;
using NUnit.Framework;
using static Contoso.Validators;

namespace Contoso.Application.Test.Validators
{
    public class StringValidationTest
    {
        [Test]
        public void NotLongerThan_PassesValidation()
        {
            // Arrange
            var notLongerThan50 = NotLongerThan(50);

            // Act
            notLongerThan50("Hello There").Match(
                Succ: s => Assert.Pass(),
                Fail: e => Assert.Fail("Validation should have passed. " + e.Join().ToString()));
        }

        [Test]
        public void NotLongerThan_FailsValidation()
        {
            // Arrange
            var notLongerThan10 = NotLongerThan(10);

            // Act
            notLongerThan10("Little by little, one travels far.").Match(
                Succ: s => Assert.Fail("Validation should have failed."),
                Fail: e => Assert.Pass());
        }

        [Test]
        public void NotEmpty_PassesValidation()
        {
            // Act
            var notEmpty = NotEmpty("Not all those who wander are lost.").Match(
                Succ: s => Assert.Pass(),
                Fail: e => Assert.Fail("Validation should have passed. " + e.Join().ToString()));
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void NotEmpty_FailsValidation(string str)
        {
            NotEmpty(str).Match(
                Succ: s => Assert.Fail("Validation should have failed."),
                Fail: e => Assert.Pass());
        }
    }
}
