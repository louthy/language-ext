using System;
using System.Linq;
using System.Reflection;
using LanguageExt;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        Asm();

        var x = new TestStruct(1, "Hello", Guid.Empty);
        var y = new TestStruct(1, "Hello", Guid.Empty);
        var z = new TestStruct(1, "Hello", Guid.NewGuid());

        var a = x == y;
        var b = x == z;
        var c = x.Equals((object)y);
        var d = x.Equals((object)z);
        var e = x.Equals(y);
        var f = x.Equals(z);
        var g = JsonConvert.SerializeObject(x);
        var h = JsonConvert.DeserializeObject<TestStruct>(g);
        var i = x.ToString();
        var j = x > y;
        var k = x < z;
        var l = x.GetHashCode();
        var m = y.GetHashCode();
        var n = z.GetHashCode();


        Console.WriteLine("Coming soon");
    }


    public class TestStruct : IEquatable<TestStruct>, IComparable<TestStruct>, ISerializable
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;

        public TestStruct(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        TestStruct(SerializationInfo info, StreamingContext context) =>
            RecordType<TestStruct>.SetObjectData(this, info);

        public void GetObjectData(SerializationInfo info, StreamingContext context) =>
            RecordType<TestStruct>.GetObjectData(this, info);

        public override int GetHashCode() =>
            RecordType<TestStruct>.Hash(this);

        public override bool Equals(object obj) =>
            RecordType<TestStruct>.Equality(this, obj);

        public int CompareTo(TestStruct other) =>
            RecordType<TestStruct>.Compare(this, other);

        public bool Equals(TestStruct other) =>
            RecordType<TestStruct>.EqualityTyped(this, other);

        public override string ToString() =>
            RecordType<TestStruct>.ToString(this);

        public static bool operator ==(TestStruct x, TestStruct y) =>
            RecordType<TestStruct>.EqualityTyped(x, y);

        public static bool operator !=(TestStruct x, TestStruct y) =>
            !(x == y);

        public static bool operator >(TestStruct x, TestStruct y) =>
            RecordType<TestStruct>.Compare(x, y) > 0;

        public static bool operator >=(TestStruct x, TestStruct y) =>
            RecordType<TestStruct>.Compare(x, y) >= 0;

        public static bool operator <(TestStruct x, TestStruct y) =>
            RecordType<TestStruct>.Compare(x, y) < 0;

        public static bool operator <=(TestStruct x, TestStruct y) =>
            RecordType<TestStruct>.Compare(x, y) <= 0;
    }

    static void Asm()
    {
        ClassInstancesAssembly.Register(typeof(Program).GetTypeInfo().Assembly);

        var f = Class<Eq<string>>.Default.Equals("1", "1");

        JObject a = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Hello", B = "World" }));
        JObject b = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Hello", B = "World" }));
        JObject c = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Different", B = "World" }));

        var i = new TestEqJObject(a);
        var j = new TestEqJObject(b);
        var k = new TestEqJObject(c);

        var x = i == j;
        var y = i == k;
        var z = j == k;

        var h1 = i.GetHashCode();
        var h2 = j.GetHashCode();
        var h3 = k.GetHashCode();
    }

    public class TestEqJObject : Record<TestEqJObject>
    {
        //[Eq(typeof(EqJObject))]
        public readonly JObject Value;

        public TestEqJObject(JObject value) => 
            Value = value;
    }

    public struct EqJObject : Eq<JObject>
    {
        public bool Equals(JObject x, JObject y) =>
            JObject.DeepEquals(x, y);

        public int GetHashCode(JObject x) =>
            x.GetHashCode();
    }
}