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
