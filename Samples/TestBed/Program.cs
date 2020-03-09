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
using System.Linq.Expressions;
using TestBed;

class Program
{
    static void Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        var t = new[] {1, 2, 3}.GetType();
        var tn = t.FullName;
        var tg = t.GetGenericArguments();
        

        var j = OrdClass.Compare(new[] {1, 2, 3}, new[] {1, 2});
        /*
        var k = fn2(new[] {1, 2, 3}, new[] {1, 2, 3});
        
        var fn1 = MakeEquals<byte>().Item2;

        var fn = MakeEquals<int>().Item2;

        var x = fn(1, 1);
        var y = fn(1, 2);
        
        MakeEquals<double>();      
        MakeEquals<decimal>();      
        MakeEquals<bool>();        // Boolean   System.Boolean
        MakeEquals<byte>();        // Byte      System.Byte
        MakeEquals<char>();        // Char      System.Char
        MakeEquals<int>();         // Int32     System.Int32
        MakeEquals<string>();      // String    System.String
        MakeEquals<Guid>();        // Guid      System.Guid
        MakeEquals<Option<int>>(); // Option`1  LanguageExt.Option`1[[System.Int32 ...]]
    */
    }




}
