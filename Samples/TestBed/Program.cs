////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Reflection;
using LanguageExt;
using LanguageExt.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LanguageExt.Interfaces;
using static LanguageExt.IO.File;
using TestBed;

class Program
{
    static Atom<string> atomTest = Atom("Hello");

    static void Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ///////////////////////////////////////////v////////////////////////////////////////////////////////////

        var option = Some(123);

        var result = option.Case switch
                     {
                         int x => x,
                         _     => 0
                     };

        var result2 = option.Case is string name 
                         ? $"Hello, {name}"
                         : "Hello stranger";

        var sum = Sum(Seq(1, 2, 3, 4, 5));
    }

    static int Sum(Seq<int> values) =>
        values.Case switch
        {
            null                 => 0,
            int x                => x,
            (int x, Seq<int> xs) => x + Sum(xs),
        };
}

