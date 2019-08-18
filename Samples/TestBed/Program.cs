////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        HashMapRemoveTest();
        HashMapRemoveTest2();


        TestSubs.Test();

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

    static void HashMapRemoveTest()
    {
        var rnd = new Random();
        var tries = 1;
        while(true)
        {
            var cnt = 1 + Math.Abs(rnd.Next() % 100000);

            Console.WriteLine($"Try: {tries} - {cnt} items");

            Seq<int> xs = default;
            HashMap<int, int> hm = default;

            for(var i = 0; i < cnt; i++)
            {
                var n = rnd.Next();
                if(!hm.ContainsKey(n))
                {
                    hm = hm.Add(n, n);
                    xs = xs.Add(n);
                }
            }

            foreach(var x in xs)
            {
                hm = hm.Remove(x);
                if(hm.ContainsKey(x))
                {
                    throw new Exception();
                }

                // Add something else to make sure add works after remove
                if (Math.Abs(rnd.Next() % 1000) < 100)
                {
                    var n = rnd.Next();
                    if (!hm.ContainsKey(n))
                    {
                        hm = hm.Add(n, n);
                    }
                }
            }
            tries++;
        }
    }

    static void HashMapRemoveTest2()
    {
        var values = new[] { 9321519, 2085595311 };

        var items = toHashMap(values.Zip(values));

        foreach (var value in values.Take(values.Length - 1))
        {
            items = items.Remove(value);
        }

        items = items.Add(2085595311, 2085595311);
        items = items.Remove(2085595311);

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


public partial struct Subs<S, A>
{
    internal readonly LanguageExt.RWS<TInt, TestBed.IO, int, S, A> __comp;
    internal Subs(LanguageExt.RWS<TInt, TestBed.IO, int, S, A> comp) => __comp = comp;
    public static Subs<S, A> Pure(A value) => new Subs<S, A>((env, state) => (value, default, state, false));
    public static Subs<S, A> Fail => new Subs<S, A>((env, state) => (default, default, default, true));
    public Subs<S, B> Map<B>(Func<A, B> f) => new Subs<S, B>(__comp.Map(f));
    public Subs<S, B> Select<B>(Func<A, B> f) => new Subs<S, B>(__comp.Map(f));
    public Subs<S, B> Bind<B>(Func<A, Subs<S, B>> f) => new Subs<S, B>(__comp.Bind(a => f(a).__comp));
    public Subs<S, B> SelectMany<B>(Func<A, Subs<S, B>> f) => new Subs<S, B>(__comp.Bind(a => f(a).__comp));
    public Subs<S, C> SelectMany<B, C>(Func<A, Subs<S, B>> bind, Func<A, B, C> project) => new Subs<S, C>(__comp.Bind(a => bind(a).__comp.Map(b => project(a, b))));
    public (TryOption<A> Value, int Output, S State) Run(TestBed.IO env, S state) => __comp.Run(env, state);
    public Subs<S, A> Filter(Func<A, bool> f) => new Subs<S, A>(__comp.Where(f));
    public Subs<S, A> Where(Func<A, bool> f) => new Subs<S, A>(__comp.Where(f));
    public Subs<S, A> Do(Action<A> f) => new Subs<S, A>(__comp.Do(f));
    public Subs<S, A> Strict() => new Subs<S, A>(__comp.Strict());
    public Seq<A> ToSeq(TestBed.IO env, S state) => __comp.ToSeq(env, state);
    public Subs<S, LanguageExt.Unit> Iter(Action<A> f) => new Subs<S, LanguageExt.Unit>(__comp.Iter(f));
    public Func<TestBed.IO, S, State> Fold<State>(State state, Func<State, A, State> f)
    {
        var self = this;
        return (env, s) => self.__comp.Fold(state, f).Run(env, s).Value.IfNoneOrFail(state);
    }

    public Func<TestBed.IO, S, bool> ForAll(Func<A, bool> f)
    {
        var self = this;
        return (env, s) => self.__comp.ForAll(f).Run(env, s).Value.IfNoneOrFail(false);
    }

    public Func<TestBed.IO, S, bool> Exists(Func<A, bool> f)
    {
        var self = this;
        return (env, s) => self.__comp.Exists(f).Run(env, s).Value.IfNoneOrFail(false);
    }
    public Subs<S, A> Local(Func<TestBed.IO, TestBed.IO> f) =>
        new Subs<S, A>(LanguageExt.Prelude.local<TInt, TestBed.IO, int, S, A>(__comp, f));

    public Subs<S, (A, B)> Listen<B>(Func<int, B> f) => new Subs<S, (A, B)>(__comp.Listen(f));
    public Subs<S, A> Censor(Func<int, int> f) => new Subs<S, A>(__comp.Censor(f));
}

public static partial class Subs
{
    public static Subs<S, A> Pure<S, A>(A value) => Subs<S, A>.Pure(value);
    public static Subs<S, A> Fail<S, A>() => Subs<S, A>.Fail;
    public static Subs<S, A> asks<S, A>(Func<TestBed.IO, A> f) => new Subs<S, A>((env, state) => (f(env), default, state, false));
    public static Subs<S, TestBed.IO> ask<S>() => new Subs<S, TestBed.IO>((env, state) => (env, default, state, false));
    public static Subs<S, S> get<S>() => new Subs<S, S>((env, state) => (state, default, state, false));
    public static Subs<S, A> gets<S, A>(Func<S, A> f) => new Subs<S, A>((env, state) => (f(state), default, state, false));
    public static Subs<S, Unit> put<S>(S value) => new Subs<S, Unit>((env, state) => (default, default, value, false));
    public static Subs<S, Unit> modify<S>(Func<S, S> f) => new Subs<S, Unit>((env, state) => (default, default, f(state), false));
    public static Subs<S, A> local<S, A>(Subs<S, A> ma, Func<TestBed.IO, TestBed.IO> f) => ma.Local(f);
    public static Subs<S, A> Pass<S, A>(this Subs<S, (A, Func<int, int>)> ma) => new Subs<S, A>(ma.__comp.Pass());
    public static Subs<S, A> pass<S, A>(Subs<S, (A, Func<int, int>)> ma) => new Subs<S, A>(ma.__comp.Pass());
    public static Subs<S, (A, B)> listen<S, A, B>(Subs<S, A> ma, Func<int, B> f) => ma.Listen(f);
    public static Subs<S, A> censor<S, A>(Subs<S, A> ma, Func<int, int> f) => ma.Censor(f);
    public static Subs<S, Unit> tell<S, A>(int what) => new Subs<S, Unit>(tell<TInt, TestBed.IO, int, S, A>(what));
}
