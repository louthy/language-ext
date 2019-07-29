using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System;
using System.Net.Http;

namespace LanguageExt.Tests
{

    public class TryOptionMonadTests
    {
        [Fact]
        public void TryOddNumber1()
        {
            var res = match(from x in OddNumberCrash(10)
                            from y in OddNumberCrash(10)
                            from z in OddNumberCrash(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 1000);
        }

        [Fact]
        public void TryOddNumber2()
        {
            var res = match(from x in OddNumberCrash(10)
                            from y in OddNumberCrash(9)
                            from z in OddNumberCrash(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 0);
        }

        [Fact]
        public void TryLinq1()
        {
            var res = match(from x in Num(10)
                            from y in Num(10)
                            from z in Num(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 1000);
        }

        [Fact]
        public void TryLinq2()
        {
            var res = match(from x in Num(10)
                            from y in Num(10)
                            from z in Num(10, false)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 0);
        }

        [Fact]
        public void TryLinq3()
        {
            var res = match(from x in Num(10, false)
                            from y in Num(10)
                            from z in Num(10)
                            select x * y * z,
                             Succ: v => v,
                             Fail: 0);

            Assert.True(res == 0);
        }

        [Fact]
        public void TryMatchSuccessTest1()
        {
            GetValue(true).Match(
                Succ: v => Assert.True(v == "Hello, World"),
                Fail: e => Assert.False(true)
            );
        }

        [Fact]
        public void TryMatchFailTest1()
        {
            GetValue(false).Match(
                Succ: v => Assert.False(true),
                Fail: e => Assert.True(e.Message == "Failed!")
            );
        }

        [Fact]
        public void FuncTryMatchSuccessTest1()
        {
            match(
                GetValue(true),
                Succ: v => Assert.True(v == "Hello, World"),
                Fail: e => Assert.False(true)
            );
        }

        [Fact]
        public void FuncTryMatchNoneTest1()
        {
            match(
                GetValue(false),
                Succ: v => Assert.False(true),
                Fail: e => Assert.True(e.Message == "Failed!")
            );
        }

        [Fact]
        public void FuncFailureTryMatchSuccessTest1()
        {
            Assert.True(
                ifFail(GetValue(true), "failed") == "Hello, World"
                );
        }

        [Fact]
        public void FuncFailureTryMatchFailTest1()
        {
            Assert.True(
                ifFail(GetValue(false), "failed") == "failed"
                );
        }

        public Try<string> GetValue(bool select) => () => 
            select
                ? "Hello, World"
                : failwith<string>("Failed!");

        public Try<int> Num(int x, bool select = true) =>
            () => select
                ? x
                : failwith<int>("Failed!");

        Try<int> OddNumberCrash(int x) => () =>
        {
            if (x % 2 == 0)
                return x;
            else
                throw new System.Exception("Any exception");
        };

        // Below is just some code to test compilation and inference

        Try<Uri> parseUri(string uri) => () =>
            new Uri(uri);

        Try<HttpClient> getClient() => () =>
            new HttpClient();

        Try<string> getContent(Uri uri, HttpClient client) => () =>
            client.GetStringAsync(uri).Result;

        Try<Lst<string>> getLines(string text) => () =>
            text.Split('\n').Freeze();

        Try<Lst<string>> getURLContent(string uri) =>
            from address in parseUri(uri)
            from result in use(
                getClient(),
                client => from content in getContent(address, client)
                          from lines in getLines(content)
                          select lines)
            select result;

        [Fact]
        public void UrlTest()
        {
            // Iterates all lines of content
            getURLContent("http://www.google.com").IterT(x => Console.WriteLine(x));

            // Counts the number of lines
            int numberOfLines = getURLContent("http://www.google.com").CountT();

            // Maps the lines to line-lengths, then sums them
            int totalSize = getURLContent("http://www.google.com")
                                .MapT(x => x.Length)
                                .SumT<TInt, int>();
        }
    }
}
