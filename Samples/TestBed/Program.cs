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
        ///////////////////////////////////////////v////////////////////////////////////////////////////////////

        Map<string, int> xs = (("w", 1), ("x", 2), ("y", 3), ("z", 4));

        var x1 = xs.ContainsKey("w");
        var x2 = xs.ContainsKey("x");
        var x3 = xs.ContainsKey("y");
        var x4 = xs.ContainsKey("z");
    }
}

[Record]
public partial class Person
{
    public readonly string Name;
    public readonly string Surname;
}

public struct OrdPerson : Ord<Person>
{
    public bool Equals(Person x, Person y) => default(EqDefault<Person>).Equals(x, y);

    public Task<bool> EqualsAsync(Person x, Person y) =>
        Equals(x, y).AsTask();
    
    public int GetHashCode(Person x) => default(EqDefault<Person>).GetHashCode(x);

    public Task<int> GetHashCodeAsync(Person x) =>
        GetHashCode(x).AsTask();
    
    private int? CompareHelper(int comparisonResult) => comparisonResult == 0 ? null : (int?) comparisonResult;

    public int Compare(Person x, Person y) =>
        CompareHelper(x.Name.CompareTo(y.Name)) ??
        CompareHelper(x.Surname.CompareTo(y.Surname)) ??
        0;
    
    public Task<int> CompareAsync(Person x, Person y) =>
        Compare(x, y).AsTask();
}

[Union]
public interface State
{
    State Idle();
    State LoggingIn(DateTime expiresAt);
    State LoggedIn(string nonce, DateTime expiresAt);
}
