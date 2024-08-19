using System;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using LanguageExt.Common;

namespace LanguageExt.Tests
{
    
    public class SerialisationTests
    {
        [Fact]
        public void SetTest()
        {
            var set = Set("test5", "test2", "test1", "test3", "test4");

            var json = JsonConvert.SerializeObject(set);

            set = JsonConvert.DeserializeObject<Set<string>>(json);
            var lst = JsonConvert.DeserializeObject<Lst<string>>(json);

            Assert.True(set.Count == 5);
            Assert.True(set.Contains("test1"));
            Assert.True(set.Contains("test2"));
            Assert.True(set.Contains("test3"));
            Assert.True(set.Contains("test4"));
            Assert.True(set.Contains("test5"));
        }

        [Fact]
        public void LstTest()
        {
            var list = List("test5", "test2", "test1", "test3", "test4");

            var json = JsonConvert.SerializeObject(list);

            list = JsonConvert.DeserializeObject<Lst<string>>(json);

            Assert.True(list.Count == 5);
            Assert.True(list[0] == "test5");
            Assert.True(list[1] == "test2");
            Assert.True(list[2] == "test1");
            Assert.True(list[3] == "test3");
            Assert.True(list[4] == "test4");
        }

        [Fact]
        public void SeqTest()
        {
            var seq = Seq("test5", "test2", "test1", "test3", "test4");

            var json = JsonConvert.SerializeObject(seq);

            seq = JsonConvert.DeserializeObject<Seq<string>>(json);

            Assert.True(seq.Count == 5);
            Assert.True(seq[0] == "test5");
            Assert.True(seq[1] == "test2");
            Assert.True(seq[2] == "test1");
            Assert.True(seq[3] == "test3");
            Assert.True(seq[4] == "test4");
        }

        [Fact]
        public void OptionTest()
        {
            var some = Some("test");
            var none = Option<string>.None;

            var someText = JsonConvert.SerializeObject(some);
            var noneText = JsonConvert.SerializeObject(none);

            var some2 = JsonConvert.DeserializeObject<Option<string>>(someText);
            var none2 = JsonConvert.DeserializeObject<Option<string>>(noneText);

            Assert.True(some == some2);
            Assert.True(none == none2);
        }
        
        [Fact]
        public void ActionTypeTest()
        {
            var x = new ActionType("Test1");
            var y = new ActionType("Test2");
            var z = new ActionType("Test3");

            Assert.False(x == y);
            Assert.True(x != y);
        }

        [Serializable]
        public record ActionType(string Value);

        [Fact]
        public void ErrorSerialisationTest()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            var error = Error.New("Test");
            var json = JsonConvert.SerializeObject(error, settings);
            var error1 = JsonConvert.DeserializeObject<Error>(json, settings);

            Assert.True(error == error1);
        }
    }
}
