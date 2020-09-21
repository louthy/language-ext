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

        var r1 = new RecordTest("Paul", "Louth");
        var r2 = new RecordTest("Paul", "Louth");

        var r = r1 == r2;
    }

    public struct Maybe<A>
    {
        Eff<A> ma;

        Maybe(Eff<A> ma) =>
            this.ma = ma;

        public Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
            new Maybe<B>(ma.Bind(a => f(a).ma));

        public Maybe<B> Map<B>(Func<A, B> f) =>
            new Maybe<B>(ma.Map(f));

        public static readonly Maybe<A> Nothing = 
            new Maybe<A>(FailEff<A>(Error.New(0, "Nothing")));

        public static Maybe<A> Just(A value) =>
            new Maybe<A>(SuccessEff(value));

        public static implicit operator Maybe<A>(Eff<A> ma) =>
            new Maybe<A>(ma);

        public static implicit operator Eff<A>(Maybe<A> ma) =>
            ma.ma;
    }

    public class RecordTest : Record<RecordTest>
    {
        public RecordTest(string name, string fullName)
        {
            Name     = name;
            FullName = fullName;
        }

        public string Name { get;  }
        public string FullName { get;  }
    }
}

