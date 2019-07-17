using System;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt;
using System.Net.Http;
using Nito.AsyncEx;

namespace LanguageExt.Tests
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
            new HttpClient().AsTask();

        Task<string> getContent(Uri uri, HttpClient client) =>
            client.GetStringAsync(uri);

        Task<Lst<string>> getLines(string text) =>
            text.Split('\n').Freeze().AsTask();

        Task<Lst<string>> getURLContent(string uri) =>
            from address in parseUri(uri)
            from result  in use(
                getClient(),
                client => from content in getContent(address, client)
                          from lines   in getLines(content)
                          select lines)
            select result;

        [Fact]
        public async Task UrlTest()
        {
            // Iterates all lines of content
            await getURLContent("http://www.google.com").IterT(x => Console.WriteLine(x));

            // Counts the number of lines
            int numberOfLines = await getURLContent("http://www.google.com").CountT();

            // Maps the lines to line-lengths, then sums them
            int totalSize = await getURLContent("http://www.google.com")
                                .MapT(x => x.Length)
                                .SumT<TInt, int>();
        }

        [Fact]
        private static async Task MTaskFold_WithTaskWaitingForActivation_DoesNotHalt()
        {
            var intTask = TimeSpan
              .FromMilliseconds(100)
              .Apply(Task.Delay)
              .ContinueWith(_ => 0);

            var actual = await default(MTask<int>).Fold(intTask, 0, (x, y) => 0)(unit);

            // execution terminates by reaching here
        }

        [Fact]
        public async Task TaskOptionBindT_InitialOptionInNoneState_NoExceptionThrown()
        {
            Option<Unit> x = None;
            var none = x.AsTask();
            var task = none.BindT(_ => none);

            await task;
        }
    }
}
