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

public static class TestExt
{

    public static IEnumerable<TryAsync<B>> Traverse2<A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, B> f)
    {
        var state = default(MEnumerable<TryAsync<B>>).Zero();

        var fold = default(MTryAsync<IEnumerable<A>>).Fold(ma, state, (outerState, enumerableA) =>
            Trans<MEnumerable<TryAsync<B>>, IEnumerable<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>.Inst.Plus(
                outerState,
                default(MEnumerable<A>).Bind<MEnumerable<TryAsync<B>>, IEnumerable<TryAsync<B>>, TryAsync<B>>(
                    enumerableA,
                    a =>
                        default(MEnumerable<TryAsync<B>>).Return(default(MTryAsync<B>).Return(f(a))))));

        return fold(unit);
    }

    public static IEnumerable<TryAsync<A>> Sequence2<A>(this TryAsync<IEnumerable<A>> ma) =>
        ma.Traverse2(identity);
}

class Program
{
    static TryAsync<IEnumerable<string>> GetNumbersPoorly2() =>
        new TryAsync<IEnumerable<string>>(() => new Result<IEnumerable<string>>(new InvalidOperationException("uhhh")).AsTask());

    static TryAsync<IEnumerable<string>> GetNumbersPoorly() =>
        new TryAsync<IEnumerable<string>>(() => throw new InvalidOperationException("uhhh"));

    static IEnumerable<TryAsync<B>> Traverse<A, B>(TryAsync<IEnumerable<A>> ma, Func<A, B> f) =>
        Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MEnumerable<A>, IEnumerable<A>, A>
            .Inst.Traverse<MEnumerable<TryAsync<B>>, IEnumerable<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

    static void TestSequence()
    {
        //var state = default(MEnumerable<TryAsync<string>>).Return(default(MTryAsync<string>).Zero());

        //var fold = default(MTryAsync<IEnumerable<string>>).Fold(GetNumbersPoorly(), state, (outerState, enumerableA) =>
        //    Trans<MEnumerable<TryAsync<string>>, IEnumerable<TryAsync<string>>, MTryAsync<string>, TryAsync<string>, string>.Inst.Plus(
        //        outerState,
        //        default(MEnumerable<string>).Bind<MEnumerable<TryAsync<string>>, IEnumerable<TryAsync<string>>, TryAsync<string>>(
        //            enumerableA, 
        //            a =>
        //                default(MEnumerable<TryAsync<string>>).Return(default(MTryAsync<string>).Return(a))))
        //);

        //var resultA = fold(unit);

        var result1 = GetNumbersPoorly()
                         .Sequence();

        result1.Match(
            ()      => { Console.WriteLine("Correct"); return unit; },
            it      =>
            {
                return it.Match(
                    Succ: res => Console.WriteLine(string.Join(',', res)),
                    Fail: ex  => Console.WriteLine(ex.ToString())
                ).Result;
            },
            (x, xs) => { Console.WriteLine("Wrong"); ; return unit; });


        var result = GetNumbersPoorly()
            .Sequence()
            .Sequence();

        result.Match(
            Succ: res => Console.WriteLine(string.Join(',', res)),
            Fail: ex => Console.WriteLine(ex.ToString())).Wait();
    }

    static void Main(string[] args)
    {
        TestSequence();

        TraverseTest();
        return;

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

    public static void TraverseTest()
    {
        List(
            TryAsync(() => 1),
            TryAsync(() => 2),
            TryAsync(() => 3),
            TryAsync(() => 4))
        .Traverse(n => n)
        .Match(Succ: res => Debug.Assert(res == List(1, 2, 3, 4)),
               Fail: ex => Debug.Fail(ex.ToString()))
        .GetAwaiter().GetResult();
    }

    public static Task<Option<Gender>> GetGenderByIdAsync(Guid id) =>
        Task.FromResult(Some(new Gender(1)));

    public static async Task<Result<Gender>> MonadicGetGenderByIdAsync(Guid id)
    {
        var program =
            from i in id.ToOption().ToTryOptionAsync()
            from g in GetGenderByIdAsync(i).ToTryOptionAsync()
            select g;

        return (await program()).Match(
            Some: gender => new Result<Gender>(gender),
            None: () => Result<Gender>.None,
            Fail: e => Result<Gender>.Bottom
        );
    }

    static void WriterTest1()
    {
        var computation = from x in Writer<MSeq<string>, Seq<string>, int>(100)
                          from y in Writer<MSeq<string>, Seq<string>, int>(200)
                          from _1 in tell<MSeq<string>, Seq<string>>(SeqOne("Hello"))
                          from _2 in tell<MSeq<string>, Seq<string>>(SeqOne("World"))
                          from _3 in tell<MSeq<string>, Seq<string>>(SeqOne($"the result is {x + y}"))
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