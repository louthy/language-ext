﻿////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.Contracts;
using System.IO;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace TestBed
{
    [Free]
    public interface FreeIO<T>
    {
        [Pure] T Pure(T value);
        [Pure] T Fail(Error error);
        string ReadAllText(string path);
        Unit WriteAllText(string path, string text);
    }
    
    [Free]
    public interface Maybe<A>
    {
        [Pure] A Just(A value);
        [Pure] Unit Nothing();

        public static Maybe<B> Map<B>(Maybe<A> ma, Func<A, B> f) => ma switch
        {
            Just<A>(var x) => Maybe.Just(f(x)),
            _              => Maybe.Nothing<B>()
        };
    }

    public static class MaybeFreeTest
    {
        public static void Test1()
        {
            var ma = Maybe.Just(10);
            var mb = Maybe.Just(20);
            var mn = Maybe.Nothing<int>();

            var mr =
                from a in ma
                from b in mb
                select a + b;

            var mnn =
                from a in ma
                from b in mb
                from _ in mn
                select a + b;
            
            Console.WriteLine(mr);
            Console.WriteLine(mnn);
        }
    }


    [WithLens]
    public partial class TestWith : Record<TestWith>
    {
        public readonly string Name;
        public readonly string Surname;

        public TestWith(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }
    }

    public interface IO
    {
        Seq<string> ReadAllLines(string fileName);
        Unit WriteAllLines(string fileName, Seq<string> lines);
        Person ReadFromDB();
        int Zero { get; }
    }

    public class RealIO : IO
    {
        public Seq<string> ReadAllLines(string path) => File.ReadAllLines(path).ToSeq();
        public Unit WriteAllLines(string path, Seq<string> lines)
        {
            File.WriteAllLines(path, lines);
            return unit;
        }
        public Person ReadFromDB() => new Person("Spider", "Man");
        public int Zero => 0;
    } 

    public static class TestSubs
    {
        public static void Test()
        {
            var comp = from ze in Subsystem.Zero
                       from ls in Subsystem.ReadAllLines("c:/test.txt")
                       from _  in Subsystem.WriteAllLines("c:/test-copy.txt", ls)
                       select ls.Count;

            var res = comp.Run(new RealIO()).IfFail(0);
        }
    }

    [Reader(Env: typeof(IO), Constructor: "Pure")]
    public partial struct Subsystem<A>
    {
    }


    //[RWS(WriterMonoid: typeof(MSeq<string>), Env: typeof(IO), State: typeof(string), 
    //     Constructor: "LiftSub",             Fail: "FailSub")]
    //public partial struct Subsys<T>
    //{
    //}


    [RWS(WriterMonoid: typeof(MSeq<string>), 
         Env:          typeof(IO), 
         State:        typeof(Person), 
         Constructor:  "Pure", 
         Fail:         "Error" )]
    public partial struct Subsys<T>
    {
    }

    [WithLens]
    public partial class TestWith2 : Record<TestWith2>
    {
        public readonly Option<string> Name;
        public readonly Option<string> Surname;

        public TestWith2(Option<string> name, Option<string> surname)
        {
            Name = name;
            Surname = surname;
        }
    }

    [WithLens]
    internal partial class TestWith3<A> : Record<TestWith3<A>> where A : class
    {
        public readonly A Value;
        public readonly Option<string> Name;
        public readonly Option<string> Surname;

        public TestWith3(A value, Option<string> name, Option<string> surname)
        {
            Value = value;
            Name = name;
            Surname = surname;
        }
    }

    [WithLens]
    public partial class TestWith4 : Record<TestWith4>
    {
        public readonly string New;
        public readonly string Class;
        public readonly string Static;
        public readonly string While;

        public TestWith4(string @new, string @class, string @static, string @while)
        {
            New = @new;
            Class = @class;
            Static = @static;
            While = @while;
        }
    }

    [Union]
    public interface MaybeUnion<A>
    {
        MaybeUnion<A> JustValue(A value);
        MaybeUnion<A> NothingValue();
    }

    //[Union]
    //public abstract partial class Maybe<A>
    //{
    //    public abstract Maybe<A> Just(A value);
    //    public abstract Maybe<A> Nothing();
    //}

    //[Union]
    //public interface Shape
    //{
    //    Shape Rectangle(float width, float length);
    //    Shape Circle(float radius);
    //    Shape Prism(float width, float height);
    //}

    [Union]
    public abstract partial class Shape<NumA, A> where NumA : struct, Num<A>
    {
        public abstract Shape<NumA, A> Rectangle(A width, A length);
        public abstract Shape<NumA, A> Circle(A radius);
        public abstract Shape<NumA, A> Prism(A width, A height);
    }

    [WithLens]
    public partial class CustomClass : Record<CustomClass>
    {
        public readonly Option<string> Value;

        public CustomClass(Option<string> value)
        {
            Value = value;
        }

        public Option<int> ValueLengthAsExpressionBodiedProperty => Value.Map(x => x.Length);

        public Option<int> ValueLengthAsGetProperty
        {
            get { return Value.Map(x => x.Length); }
        }

        public Option<int> ValueLengthAsExpressionBodiedMethod() => Value.Map(x => x.Length);
    }

    [Record]
    public partial struct Person
    {
        [Eq(typeof(EqStringOrdinalIgnoreCase))]
        [Ord(typeof(OrdStringOrdinalIgnoreCase))]
        [Hashable(typeof(HashableStringOrdinalIgnoreCase))]
        public readonly string Forename;

        [Eq(typeof(EqStringOrdinalIgnoreCase))]
        [Ord(typeof(OrdStringOrdinalIgnoreCase))]
        [Hashable(typeof(HashableStringOrdinalIgnoreCase))]
        public readonly string Surname;
    }
}


