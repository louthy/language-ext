using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;

namespace TestBed
{
    [With]
    public class TestWith : Record<TestWith>
    {
        public string Name;
        public string Surname;

        public TestWith(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }
    }
}
