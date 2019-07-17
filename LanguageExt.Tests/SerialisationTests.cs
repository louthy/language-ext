using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LanguageExt.Tests
{
    
    public class SerialisationTests
    {
        [Fact]
        public void MapTest()
        {
            var map = Map(
                ("test5", "/root/user/test5"),
                ("test2", "/root/user/test2"),
                ("test1", "/root/user/test1"),
                ("test3", "/root/user/test3"),
                ("test4", "/root/user/test4")
                );

            var json = JsonConvert.SerializeObject(map);

            map = JsonConvert.DeserializeObject<Map<string, string>>(json);

            Assert.True(map.Count == 5);
            Assert.True(map.ContainsKey("test1"));
            Assert.True(map.ContainsKey("test2"));
            Assert.True(map.ContainsKey("test3"));
            Assert.True(map.ContainsKey("test4"));
            Assert.True(map.ContainsKey("test5"));
            Assert.True(map["test1"] == "/root/user/test1");
            Assert.True(map["test2"] == "/root/user/test2");
            Assert.True(map["test3"] == "/root/user/test3");
            Assert.True(map["test4"] == "/root/user/test4");
            Assert.True(map["test5"] == "/root/user/test5");
        }

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
        public void OptionUnsafeTest()
        {
            var some = SomeUnsafe("test");
            var none = OptionUnsafe<string>.None;

            var someText = JsonConvert.SerializeObject(some);
            var noneText = JsonConvert.SerializeObject(none);

            var some2 = JsonConvert.DeserializeObject<OptionUnsafe<string>>(someText);
            var none2 = JsonConvert.DeserializeObject<OptionUnsafe<string>>(noneText);

            Assert.True(some == some2);
            Assert.True(none == none2);
        }

        [Fact]
        public void EitherTest()
        {
            var right = Right<string, string>("test");
            var left = Left<string, string>("error");

            var rightText = JsonConvert.SerializeObject(right);
            var leftText = JsonConvert.SerializeObject(left);

            var right2 = JsonConvert.DeserializeObject<Either<string, string>>(rightText);
            var left2 = JsonConvert.DeserializeObject<Either<string, string>>(leftText);

            Assert.True(right == right2);
            Assert.True(left == left2);
        }

        [Fact]
        public void EitherUnsafeTest()
        {
            var right = RightUnsafe<string, string>("test");
            var left = LeftUnsafe<string, string>("error");

            var rightText = JsonConvert.SerializeObject(right);
            var leftText = JsonConvert.SerializeObject(left);

            var right2 = JsonConvert.DeserializeObject<EitherUnsafe<string, string>>(rightText);
            var left2 = JsonConvert.DeserializeObject<EitherUnsafe<string, string>>(leftText);

            Assert.True(right == right2);
            Assert.True(left == left2);
        }

        [Fact]
        public void ValidationTest()
        {
            var succ = Success<string, string>("test");
            var fail = Fail<string, string>("error");

            var succText = JsonConvert.SerializeObject(succ);
            var failText = JsonConvert.SerializeObject(fail);

            var succ2 = JsonConvert.DeserializeObject<Validation<string, string>>(succText);
            var fail2 = JsonConvert.DeserializeObject<Validation<string, string>>(failText);

            Assert.True(succ == succ2);
            Assert.True(fail == fail2);
        }

        [Fact]
        public void ActionTypeTest()
        {
            var x = ActionType.New("Test1");
            var y = ActionType.New("Test2");
            var z = ActionType.New("Test3");

            Assert.False(x == y);
            Assert.False(x > y);
            Assert.True(x < y);
            Assert.True(x != y);
        }

        [Serializable]
        public class ActionType : NewType<ActionType, string>, ISerializable
        {
            public ActionType(string value) : base(value) { }
            protected ActionType(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}
