using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;

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
    }

    [Reader(typeof(IO))]
    public partial struct Subsystem<A>
    {

    }
}
