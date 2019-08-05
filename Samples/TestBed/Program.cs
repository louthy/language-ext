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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBed;

public static class Global
{
    public static Option<Guid> ToOption(this Guid id) =>
        id == default(Guid) ? Option<Guid>.None : Some(id);
}

public class Gender : NewType<Gender, int>
{
    public Gender(int value) : base(value)
    {
    }
}

public interface IRepository
{
    Task<Option<Gender>> GetGenderByIdAsync(Guid id);
}



class Program
{
    static void Main(string[] args)
    {

        SeqPerf.Broken1();
        return;
        HashMapPerf.Run();
        SeqPerf.Run();

        Test533();

        IEnumerableOptionBindT_NotEnumerabled_NotEvaluated();

        var xs = (new[] { 1, 2, 3 }).ConcatFast(new[] { 4, 5, 6 });
        var ys = (new int [0]).ConcatFast(new[] { 4, 5, 6 });
        var zs = (new[] { 1, 2, 3 }).ConcatFast(new int[0]);

        var nxs = ((IEnumerable<int>)null).ConcatFast(new[] { 4, 5, 6 });
        var nzs = (new[] { 1, 2, 3 }).ConcatFast(((IEnumerable<int>)null));

        var bxs = new[] { 1, 2, 3 }.Bind(va => new[] { 1 * va, 2 * va, 3 * va });


        var xyz = new TestWith("Paul", "Louth");
        var xyz2 = new TestWith2("Paul", "Louth");
        var xyz3 = new TestWith3<string>("Test", "Paul", "Louth");
        var xyz4 = new TestWith4("", "", "", "");

        //TestWith4.@new.Set

        xyz3 = xyz3.With(Value: "Another");

        xyz2 = xyz2.With();
        xyz2 = xyz2.With(Name: Some("P"));
        xyz2 = xyz2.With(Surname: Some("L"));
        xyz2 = xyz2.With(Name: None, Surname: None);

        xyz = xyz.With(Name: "Test1");
        xyz = TestWith.surname.Set("Test2", xyz);


        var asq = typeof(LanguageExt.CodeGen.RecordWithAndLensGenerator).AssemblyQualifiedName;

        var r = MonadicGetGenderByIdAsync(Guid.NewGuid()).Result;

        WriterTest1();
        WriterTest2();
        WriterTest3();
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

    public static void Test533()
    {

        var someData = Enumerable
            .Range(0, 30000)
            .Select(_ => Guid.NewGuid().ToString())
            .ToArray();

        var result = someData
            .Select(Some)
            .Sequence()
            .Map(x => x.ToArray());
    }

    public static void IEnumerableOptionBindT_NotEnumerabled_NotEvaluated()
    {
        var evaluted = false;
        var list = Seq1(unit)
            .AsEnumerable()
            .Select(_ =>
            {
                evaluted = true;
                return unit;
            })
            .Select(Some)
            .BindT(Some);

        Debug.Assert(!evaluted);
    }

    public static void TraverseTest()
    {
        List(
            TryAsync(1.AsTask()),
            TryAsync(2.AsTask()),
            TryAsync(3.AsTask()),
            TryAsync(4.AsTask()))
        .Traverse(n => n)
        .Match(Succ: res => Debug.Assert(res == List(1, 2, 3, 4)),
               Fail: ex => Debug.Fail(ex.ToString()))
        .GetAwaiter().GetResult();
    }

    public static Task<Option<Gender>> GetGenderByIdAsync(Guid id) =>
        Some(new Gender(1)).AsTask();

    public static async Task<Result<Gender>> MonadicGetGenderByIdAsync(Guid id)
    {
        var program =
            from i in id.ToOption().ToTryOptionAsync()
            from g in GetGenderByIdAsync(i).ToTryOptionAsync()
            select g;

        return await program.Match(
            Some: gender => new Result<Gender>(gender),
            None: () => Result<Gender>.Bottom,
            Fail: e => Result<Gender>.Bottom
        );
    }

    static void WriterTest1()
    {
        var computation = from x in Writer<MSeq<string>, Seq<string>, int>(100)
                          from y in Writer<MSeq<string>, Seq<string>, int>(200)
                          from _1 in tell<MSeq<string>, Seq<string>>(Seq1("Hello"))
                          from _2 in tell<MSeq<string>, Seq<string>>(Seq1("World"))
                          from _3 in tell<MSeq<string>, Seq<string>>(Seq1($"the result is {x + y}"))
                          select x + y;

        var result = computation();

        Debug.Assert(result.Value == 300);
        Debug.Assert(result.Output.Count == 3);
        Debug.Assert(String.Join(" ", result.Output) == "Hello World the result is 300");
    }

    static void WriterTest2()
    {
        var computation = from _1 in tell<TInt, int>(2)
                          from _2 in tell<TInt, int>(4)
                          from _3 in tell<TInt, int>(6)
                          select unit;

        var result = computation();

        Debug.Assert(result.Value == unit);
        Debug.Assert(result.Output == 12);
    }

    struct MProduct : Monoid<int>
    {
        public int Append(int x, int y) =>
            x * y;

        public int Empty() =>
            1;
    }

    static void WriterTest3()
    {
        var computation = from _1 in tell<MProduct, int>(2)
                          from _2 in tell<MProduct, int>(4)
                          from _3 in tell<MProduct, int>(6)
                          select unit;

        var result = computation();

        Debug.Assert(result.Value == unit);
        Debug.Assert(result.Output == 48);
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

        var ma = Class<Monad<Option<string>, string>>.Default;

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
