using System;
using LanguageExt;

namespace LibraryWithCodeGen
{
    [Record]
    public partial struct Device
    {
        public readonly string Name;
        public readonly int Id;
    }
}
