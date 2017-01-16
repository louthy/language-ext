using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using Xunit;
using Newtonsoft.Json;

namespace LanguageExtTests
{
    
    public class SerialisationTests
    {
        [Fact]
        public void ProcessIdTest()
        {
            ProcessId pid = "/root/user/test";

            Assert.True(pid.Path == "/root/user/test");
            Assert.True(pid.Name.Value == "test");
            Assert.True(pid.Child("ing").Path == "/root/user/test/ing");

            var json = JsonConvert.SerializeObject(pid);

            pid = JsonConvert.DeserializeObject<ProcessId>(json);

            Assert.True(pid.Path == "/root/user/test");
            Assert.True(pid.Name.Value == "test");
            Assert.True(pid.Child("ing").Path == "/root/user/test/ing");
        }

        [Fact]
        public void ProcessNameTest()
        {
            ProcessName name = "test";

            Assert.True(name.Value == "test");

            var json = JsonConvert.SerializeObject(name);

            name = JsonConvert.DeserializeObject<ProcessName>(json);

            Assert.True(name.Value == "test");
        }

        [Fact]
        public void MapTest()
        {
            var map = Map(
                Tuple<ProcessName,ProcessId>("test5", "/root/user/test5"),
                Tuple<ProcessName, ProcessId>("test2", "/root/user/test2"),
                Tuple<ProcessName, ProcessId>("test1", "/root/user/test1"),
                Tuple<ProcessName, ProcessId>("test3", "/root/user/test3"),
                Tuple<ProcessName, ProcessId>("test4", "/root/user/test4")
                );

            var json = JsonConvert.SerializeObject(map);

            map = JsonConvert.DeserializeObject<Map<ProcessName, ProcessId>>(json);

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
            var set = Set<ProcessName>("test5", "test2", "test1", "test3", "test4");

            var json = JsonConvert.SerializeObject(set);

            set = JsonConvert.DeserializeObject<Set<ProcessName>>(json);
            var lst = JsonConvert.DeserializeObject<Lst<ProcessName>>(json);

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
            var list = List<ProcessName>("test5", "test2", "test1", "test3", "test4");

            var json = JsonConvert.SerializeObject(list);

            list = JsonConvert.DeserializeObject<Lst<ProcessName>>(json);

            Assert.True(list.Count == 5);
            Assert.True(list[0] == "test5");
            Assert.True(list[1] == "test2");
            Assert.True(list[2] == "test1");
            Assert.True(list[3] == "test3");
            Assert.True(list[4] == "test4");
        }
        [Fact]
        public void OptionTest()
        {
            var some = Some((ProcessName)"test");
            var none = Option<ProcessName>.None;

            var someText = JsonConvert.SerializeObject(some);
            var noneText = JsonConvert.SerializeObject(none);

            var some2 = JsonConvert.DeserializeObject<Option<ProcessName>>(someText);
            var none2 = JsonConvert.DeserializeObject<Option<ProcessName>>(noneText);

            Assert.True(some == some2);
            Assert.True(none == none2);
        }

        [Fact]
        public void OptionUnsafeTest()
        {
            var some = SomeUnsafe((ProcessName)"test");
            var none = OptionUnsafe<ProcessName>.None;

            var someText = JsonConvert.SerializeObject(some);
            var noneText = JsonConvert.SerializeObject(none);

            var some2 = JsonConvert.DeserializeObject<OptionUnsafe<ProcessName>>(someText);
            var none2 = JsonConvert.DeserializeObject<OptionUnsafe<ProcessName>>(noneText);

            Assert.True(some == some2);
            Assert.True(none == none2);
        }

        [Fact]
        public void EitherTest()
        {
            var right = Right<string, ProcessName>("test");
            var left = Left<string, ProcessName>("error");

            var rightText = JsonConvert.SerializeObject(right);
            var leftText = JsonConvert.SerializeObject(left);

            var right2 = JsonConvert.DeserializeObject<Either<string, ProcessName>>(rightText);
            var left2 = JsonConvert.DeserializeObject<Either<string, ProcessName>>(leftText);

            Assert.True(right == right2);
            Assert.True(left == left2);
        }

        [Fact]
        public void EitherUnsafeTest()
        {
            var right = RightUnsafe<string, ProcessName>("test");
            var left = LeftUnsafe<string, ProcessName>("error");

            var rightText = JsonConvert.SerializeObject(right);
            var leftText = JsonConvert.SerializeObject(left);

            var right2 = JsonConvert.DeserializeObject<EitherUnsafe<string, ProcessName>>(rightText);
            var left2 = JsonConvert.DeserializeObject<EitherUnsafe<string, ProcessName>>(leftText);

            Assert.True(right == right2);
            Assert.True(left == left2);
        }
    }
}
