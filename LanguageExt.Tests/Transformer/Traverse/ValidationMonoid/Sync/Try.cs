﻿using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.ValidationMonoid.Sync
{
    public class Try
    {
        [Fact]
        public void TrySuccessIsSuccessTry()
        {
            var ma = Try(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Try<int>>(Try(12));

            var eq = mb == mc;
            Assert.True(eq);
        }

        [Fact]
        public void TryFailisFailTry()
        {
            var ma = Try<Validation<MSeq<Error>, Seq<Error>, int>>(new Exception("Fail"));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Try<int>>(Try<int>(new Exception("Fail")));

            var eq = mb == mc;
            Assert.True(eq);
        }

        [Fact]
        public void TryValidationFailIsValidationFailTry()
        {
            var ma = TrySucc(Success<MSeq<Error>, Seq<Error>, int>(12));
            var mb = ma.Traverse(identity);
            var mc = Success<MSeq<Error>, Seq<Error>, Try<int>>(TrySucc(12));

            var eq = mb == mc;
            Assert.True(eq);
        }
    }
}
