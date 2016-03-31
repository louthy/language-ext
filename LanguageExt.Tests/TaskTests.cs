using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static LanguageExt.Prelude;
using LanguageExt;
using LanguageExt.Trans;
using System.Net;
using System.Collections.Generic;

namespace LanguageExtTests
{
    public class TaskTests
    {
        [Fact]
        public void TaskLINQTest1()
        {
            var computation = from x   in Action1(0)
                              from y   in Action2(x)
                              from opt in Action3(y)
                              select opt.Map(z => x + y + z);

            Assert.True(computation.Result == 10);
        }

        [Fact]
        public void TaskLINQTest2()
        {
            var computation = from x in Action1(0)
                              from y in Action2(x)
                              from opt in Action3(y)
                              select from z in opt 
                                     select x + y + z;

            Assert.True(computation.Result == 10);
        }

        public async Task<int> Action1(int x)
        {
            await Task.Delay(100);
            return x + 1;
        }

        public async Task<int> Action2(int x)
        {
            await Task.Delay(200);
            return x + 2;
        }

        public async Task<Option<int>> Action3(int x)
        {
            await Task.Delay(300);
            return x + 3;
        }

        public async Task<int> Action4(int x)
        {
            await Task.Delay(400);
            return x + 4;
        }

        Task<Uri> parseUri(string uri) => 
            new Uri(uri).AsTask();

        Task<WebRequest> openConnection(Uri uri) =>
            WebRequest.CreateDefault(uri).AsTask();

        Task<WebResponse> getInputStream(WebRequest req) =>
            req.GetResponseAsync();

        Task<Stream> getSource(WebResponse resp) =>
            resp.GetResponseStream().AsTask();

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

        Task<IEnumerable<string>> getLines(Stream stream) =>
            readAllLines(stream).ToList().AsEnumerable().AsTask();

        public Task<IEnumerable<string>> getURLContent(string url) =>
            from u in parseUri(url)
            from conn in openConnection(u)
            from result in use(
                getInputStream(conn),
                stream => use(getSource(stream), getLines)
                )
            select result;

        [Fact]
        public void UrlTest()
        {
            getURLContent("http://www.google.com").IterT(x => Console.WriteLine(x));
        }
    }
}
