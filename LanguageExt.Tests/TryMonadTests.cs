using Xunit;
using LanguageExt;
using System.IO;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.Trans;
using System;
using System.Net;

namespace LanguageExtTests
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

        public Try<string> GetValue(bool select) =>
            () => select
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

        Try<WebRequest> openConnection(Uri uri) => () =>
            WebRequest.CreateDefault(uri);

        Try<WebResponse> getInputStream(WebRequest req) => () =>
            req.GetResponse();

        Try<Stream> getSource(WebResponse resp) => () =>
            resp.GetResponseStream();

        IEnumerable<string> readAllLines(Stream stream)
        {
            List<char> cs = new List<char>();
            while (true)
            {
                int b = stream.ReadByte();
                if (b == -1 || b == 0) yield break;

                if (b == 13)
                {
                    yield return new String(cs.ToArray());
                    cs.Clear();
                }
                if (b > 30) cs.Add((char)b);
            }
        }

        Try<IEnumerable<string>> getLines(Stream stream) => () =>
            TryResult.Cast(readAllLines(stream));

        public Try<IEnumerable<string>> getURLContent(string url) =>
            from u      in parseUri(url)
            from conn   in openConnection(u)
            from stream in use(getInputStream(conn))
            from source in use(getSource(stream.Value))
            from lines  in getLines(source.Value)
            from line   in lines
            select line;

        [Fact]
        public void UrlTest()
        {
            getURLContent("http://www.google.com").IterT(x => Console.WriteLine(x));
        }
    }
}
