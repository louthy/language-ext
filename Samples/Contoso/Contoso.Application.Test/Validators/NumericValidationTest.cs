using Contoso.Core;
using LanguageExt;
using NUnit.Framework;
using static Contoso.Validators;

namespace Contoso.Application.Test.Validators
{
    public class NumericValidationTest
    {
        [Test]
        public void AtLeast_PassesValidation()
        {
            // Arrange
            var atLeast50 = AtLeast(50M);

            // Act
            atLeast50(1111M).Match(
                Succ: d => Assert.Pass(),
                Fail: e => Assert.Fail("Validation should have passed. " + e.Join().ToString()));
        }

        [Test]
        public void AtLeast_FailsValidation()
        {
            // Arrange
            var atLeast100 = AtLeast(100M);

            // Act
            atLeast100(10M).Match(
                Succ: d => Assert.Fail("Validation should have failed."),
                Fail: e => Assert.Pass());
        }

        [Test]
        public void AtMost_PassesValidation()
        {
            // Arrange
            var atMost100 = AtMost(100);

            // Act
            atMost100(100).Match(
                Succ: d => Assert.Pass(),
                Fail: e => Assert.Fail("Validation should have passed. " + e.Join().ToString()));
        }

        [Test]
        public void AtMost_FailsValidation()
        {
            // Arrange
            var atMost10 = AtMost(10M);

            atMost10(10.000000001M).Match(
                Succ: d => Assert.Fail("Validation should have failed"),
                Fail: e => Assert.Pass());
        }
    }
}
