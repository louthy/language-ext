using System;

using Xunit;

namespace LanguageExt.Tests
{
    public class ResultTests
    {
        [Fact]
        public void TestBottom()
        {
            Assert.True(_bottomResult.IsFaulted);
            Assert.True(_bottomResult.IsBottom);
        }

        [Fact]
        public void TestSuccess()
        {
            Assert.False(_successResult.IsFaulted);
            Assert.False(_successResult.IsBottom);
        }

        [Fact]
        public void TestFault()
        {
            Assert.True(_faultResult.IsFaulted);
            Assert.False(_faultResult.IsBottom);
        }

        [Fact]
        public void TestFaultWithNullException()
        {
            Assert.True(_faultWithNullException.IsFaulted);
            Assert.False(_faultWithNullException.IsBottom);
        }

        [Fact]
        public void TestMatchWithBottom()
        {
            string output = _bottomResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestMatchWithSuccess()
        {
            string output = _successResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Success", output);
        }

        [Fact]
        public void TestMatchWithSuccessReturnsValue()
        {
            int output = _successResult.Match(
                Succ: v => v,
                Fail: ex => -1
            );
            
            Assert.Equal(1, output);
        }

        [Fact]
        public void TestMatchWithFault()
        {
            string output = _faultResult.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestMatchWithFaultWithNullException()
        {
            string output = _faultWithNullException.Match(
                Succ: _ => "Success",
                Fail: _ => "Failure"
            );

            Assert.Equal("Failure", output);
        }

        [Fact]
        public void TestTransitiveMatchWithBottom()
        {
            var output = _bottomResult.Match(
                Succ: v => v,
                Fail: ex => new Result<int>(ex)
            );

            Assert.True(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestIfFailDefaultValueWithBottom()
        {
            var output = _bottomResult.IfFail(defaultValue: 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailDefaultValueWithSuccess()
        {
            var output = _successResult.IfFail(defaultValue: 2);

            Assert.Equal(output, 1);
        }

        [Fact]
        public void TestIfFailDefaultValueWithFault()
        {
            var output = _faultResult.IfFail(defaultValue: 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailDefaultValueWithFaultWithNullException()
        {
            var output = _faultWithNullException.IfFail(defaultValue: 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailFuncWithBottom()
        {
            var output = _bottomResult.IfFail(_ => 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailFuncWithSuccess()
        {
            var output = _successResult.IfFail(_ => 2);

            Assert.Equal(output, 1);
        }

        [Fact]
        public void TestIfFailFuncWithFault()
        {
            var output = _faultResult.IfFail(_ => 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailFuncWithFaultWithNullException()
        {
            var output = _faultWithNullException.IfFail(_ => 2);

            Assert.Equal(output, 2);
        }

        [Fact]
        public void TestIfFailActionWithBottom()
        {
            bool called = false;
            _bottomResult.IfFail(_ => called = true);

            Assert.True(called);
        }

        [Fact]
        public void TestIfFailActionWithSuccess()
        {
            bool called = false;
            _successResult.IfFail(_ => called = true);

            Assert.False(called);
        }

        [Fact]
        public void TestIfFailActionWithFault()
        {
            bool called = false;
            _faultResult.IfFail(_ => called = true);

            Assert.True(called);
        }

        [Fact]
        public void TestIfFailActionWithFaultWithNullException()
        {
            bool called = false;
            _faultWithNullException.IfFail(_ => called = true);

            Assert.True(called);
        }
        
        [Fact]
        public void TestIfSuccWithBottom()
        {
            bool called = false;
            _bottomResult.IfSucc(_ => called = true);
            
            Assert.False(called);
        }

        [Fact]
        public void TestIfSuccWithSuccess()
        {
            bool called = false;
            _successResult.IfSucc(_ => called = true);
            
            Assert.True(called);
        }

        [Fact]
        public void TestIfSuccWithFault()
        {
            bool called = false;
            _faultResult.IfSucc(_ => called = true);
            
            Assert.False(called);
        }

        [Fact]
        public void TestIfSuccWithFaultWithNullException()
        {
            bool called = false;
            _faultWithNullException.IfSucc(_ => called = true);

            Assert.False(called);
        }
        
        [Fact]
        public void TestMapWithBottom()
        {
            var output = _bottomResult.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestMapWithSuccess()
        {
            var output = _successResult.Map(_ => 2);
            
            Assert.False(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestMapWithSuccessReturnsValue()
        {
            var output = _successResult.Map(_ => 2);

            var value = output.Match(
                Succ: v => v,
                Fail: _ => int.MaxValue
            );

            Assert.Equal(2, value);
        }

        [Fact]
        public void TestMapWithFault()
        {
            var output = _faultResult.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        [Fact]
        public void TestMapWithFaultWithNullException()
        {
            var output = _faultWithNullException.Map(_ => 2);
            
            Assert.True(output.IsFaulted);
            Assert.False(output.IsBottom);
        }

        private readonly Result<int> _bottomResult = new Result<int>();
        private readonly Result<int> _successResult = new Result<int>(1);
        private readonly Result<int> _faultResult = new Result<int>(new InvalidOperationException());
        private readonly Result<int> _faultWithNullException = new Result<int>((Exception) null);
    }
}