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
}
