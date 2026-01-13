////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed;

public static class RangeTests
{
    public static void Run()
    {
        var r = Range(0, 0);
        var a = r.ToArr();
        
        Console.WriteLine(a);
    }
}
