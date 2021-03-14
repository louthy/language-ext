using System;
using LanguageExt.Common;

using Xunit;

namespace LanguageExt.Tests
{
    public class ResultTests
    {
        [Fact]
        public void TestBottom()
        {
            Assert.True(bottomResult.IsFaulted);
            Assert.True(bottomResult.IsBottom);
        }

        [Fact]
        public void TestSuccess()
        {
            Assert.False(successResult.IsFaulted);
            Assert.False(successResult.IsBottom);
        }

        [Fact]
        public void TestFault()
        {
            Assert.True(faultResult.IsFaulted);
            Assert.False(faultResult.IsBottom);
        }

        [Fact]
        public void TestFaultWithNullException()
        {
            Assert.True(faultWithNullException.IsFaulted);
            Assert.True(faultWithNullException.IsBottom);
        }

        [Fact]
        public void TestMatchWithBottom()
        {
            string output = bottomResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestMatchWithSuccess()
        {
            string output = successResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public void TestMatchWithSuccessReturnsValue()
        {
            int output = successResult.Match(
                Succ: v => v,
                Fail: ex => -1
            );
            
            Assert.Equal(1, output);
        }

        [Fact]
        public void TestMatchWithFault()
        {
            string output = faultResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestMatchWithFaultWithNullException()
        {
            string output = faultWithNullException.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestTransitiveMatchWithBottom()
        {
            var output = bottomResult.Match(
                Succ: v => v,
                Fail: ex => new Result<int>(ex)
            );

            Assert.True(output.IsFaulted);
            Assert.True(output.IsBottom);
        }

        [Fact]
        public void TestIfFailDefaultValueWithBottom()
        {
            var output = bottomResult.IfFail(defaultValue: 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailDefaultValueWithSuccess()
        {
            var output = successResult.IfFail(defaultValue: 2);

            Assert.Equal(1, output);
        }

        [Fact]
        public void TestIfFailDefaultValueWithFault()
        {
            var output = faultResult.IfFail(defaultValue: 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailDefaultValueWithFaultWithNullException()
        {
            var output = faultWithNullException.IfFail(defaultValue: 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailFuncWithBottom()
        {
            var output = bottomResult.IfFail(_ => 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailFuncWithSuccess()
        {
            var output = successResult.IfFail(_ => 2);

            Assert.Equal(1, output);
        }

        [Fact]
        public void TestIfFailFuncWithFault()
        {
            var output = faultResult.IfFail(_ => 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailFuncWithFaultWithNullException()
        {
            var output = faultWithNullException.IfFail(_ => 2);

            Assert.Equal(2, output);
        }

        [Fact]
        public void TestIfFailActionWithBottom()
        {
            bool called = false;
            bottomResult.IfFail(_ => called = true);

            Assert.True(called);
        }

        [Fact]
        public void TestIfFailActionWithSuccess()
        {
            bool called = false;
            successResult.IfFail(_ => called = true);

            Assert.False(called);
        }

        [Fact]
        public void TestIfFailActionWithFault()
        {
            bool called = false;
            faultResult.IfFail(_ => called = true);

            Assert.True(called);
        }

        [Fact]
        public void TestIfFailActionWithFaultWithNullException()
        {
            bool called = false;
            faultWithNullException.IfFail(_ => called = true);

            Assert.True(called);
        }
        
        [Fact]
        public void TestIfSuccWithBottom()
        {
            bool called = false;
            bottomResult.IfSucc(_ => called = true);
            
            Assert.False(called);
        }

        [Fact]
        public void TestIfSuccWithSuccess()
        {
            bool called = false;
            successResult.IfSucc(_ => called = true);
            
            Assert.True(called);
        }

        [Fact]
        public void TestIfSuccWithFault()
        {
            bool called = false;
            faultResult.IfSucc(_ => called = true);
            
            Assert.False(called);
        }

        [Fact]
        public void TestIfSuccWithFaultWithNullException()
        {
            bool called = false;
            faultWithNullException.IfSucc(_ => called = true);

            Assert.False(called);
        }
        
        [Fact]
        public void TestMapWithBottom()
        {
            var output = bottomResult.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.True(output.IsBottom);
        }

        [Fact]
        public void TestMapWithSuccess()
        {
            var output = successResult.Map(_ => 2);
            
            Assert.False(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestMapWithSuccessReturnsValue()
        {
            var output = successResult.Map(_ => 2);

            var value = output.Match(
                Succ: v => v,
                Fail: _ => int.MaxValue
            );

            Assert.Equal(2, value);
        }

        [Fact]
        public void TestMapWithFault()
        {
            var output = faultResult.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestMapWithFaultWithNullException()
        {
            var output = faultWithNullException.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.True(output.IsBottom);
        }

        private readonly Result<int> bottomResult = default;
        private readonly Result<int> successResult = new Result<int>(1);
        private readonly Result<int> faultResult = new Result<int>(new InvalidOperationException());
        private readonly Result<int> faultWithNullException = new Result<int>((Exception) null);
    }
}
