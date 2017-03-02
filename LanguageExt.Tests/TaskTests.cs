using System;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt;
using System.Net.Http;

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

        Task<HttpClient> getClient() =>
            Task.FromResult(new HttpClient());

        Task<string> getContent(Uri uri, HttpClient client) =>
            client.GetStringAsync(uri);

        Task<Lst<string>> getLines(string text) =>
            Task.FromResult(text.Split('\n').Freeze());

        Task<Lst<string>> getURLContent(string uri) =>
            from address in parseUri(uri)
            from result  in use(
                getClient(),
                client => from content in getContent(address, client)
                          from lines   in getLines(content)
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
