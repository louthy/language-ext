using System.Linq;
using Newtonsoft.Json;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class VersionHashMapTests
    {
        [Fact]
        public void Write_with_no_conflicts()
        {
            var data = VersionHashMap<string, int>.Empty;

            data.Update(data["a"].Write("paul", 1));
            data.Update(data["b"].Write("james", 1));
            data.Update(data["b"].Write("james", 2));
            data.Update(data["b"].Write("gavin", 3));

            var str = data.AsEnumerable().OrderBy(p => p.Key).ToSeq().ToString();

            Assert.True(str == "[(a, 1), (b, 3)]");
        }
        
        [Fact]
        public void Write_with_conflicts_that_are_resolved_to_last_write_wins1()
        {
            var data = VersionHashMap<string, int>.Empty;

            var client1 = data["a"].Write("paul", 10000, 1);
            var client2 = data["a"].Write("james", 5000, 2);
            var client3 = data["b"].Write("gavin", 500, 3);

            data.Update(client1);
            data.Update(client2);
            data.Update(client3);

            var str = data.AsEnumerable().OrderBy(p => p.Key).ToSeq().ToString();

            Assert.True(str == "[(a, 1), (b, 3)]");
        }
        
        [Fact]
        public void Write_with_conflicts_that_are_resolved_to_last_write_wins2()
        {
            var data = VersionHashMap<string, int>.Empty;

            var client1 = data["a"].Write("paul", 10000, 1);
            var client2 = data["a"].Write("james", 5000, 2);
            var client3 = data["b"].Write("gavin", 500, 3);

            data.Update(client2);
            data.Update(client1);
            data.Update(client3);

            var str = data.AsEnumerable().OrderBy(p => p.Key).ToSeq().ToString();

            Assert.True(str == "[(a, 1), (b, 3)]");
        }
        
        [Fact]
        public void Write_with_conflicts_that_are_resolved_to_first_write_wins1()
        {
            var data = VersionHashMap<FirstWriteWins<int>, string, int>.Empty;

            var client1 = data["a"].Write("paul", 10000, 1);
            var client2 = data["a"].Write("james", 5000, 2);
            var client3 = data["b"].Write("gavin", 500, 3);

            data.Update(client1);
            data.Update(client2);
            data.Update(client3);

            var str = data.AsEnumerable().OrderBy(p => p.Key).ToSeq().ToString();

            Assert.True(str == "[(a, 2), (b, 3)]");
        }
        
        [Fact]
        public void Write_with_conflicts_that_are_resolved_to_first_write_wins2()
        {
            var data = VersionHashMap<FirstWriteWins<int>, string, int>.Empty;

            var client1 = data["a"].Write("paul", 10000, 1);
            var client2 = data["a"].Write("james", 5000, 2);
            var client3 = data["b"].Write("gavin", 500, 3);

            data.Update(client2);
            data.Update(client1);
            data.Update(client3);

            var str = data.AsEnumerable().OrderBy(p => p.Key).ToSeq().ToString();

            Assert.True(str == "[(a, 2), (b, 3)]");
        }
    }
}
