////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace TestBed
{
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

            var res = comp.Run(new RealIO()).IfNoneOrFail(0);
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

    public partial class Person
    {
        public readonly string Name;
        public readonly string Surname;

        public Person(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }
    }

    [RWS(WriterMonoid: typeof(MSeq<string>), Env: typeof(IO), State: typeof(Person), Constructor: "Pure", Fail: "Error")]
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
}


