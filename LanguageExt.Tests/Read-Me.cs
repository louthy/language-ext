using LanguageExt.ClassInstances;

namespace LanguageExt.Tests;

public class ReadMe
{
    public void ReadMeCode()
    {
        var abc   = ('a', 'b').Add('c');                 // ('a', 'b', 'c')
        var abcd  = ('a', 'b').Add('c').Add('d');        // ('a', 'b', 'c', 'd')
        var abcd5 = ('a', 'b').Add('c').Add('d').Add(5); // ('a', 'b', 'c', 'd', 5)

        var sumA = (1, 2, 3).Sum<TInt, int>();                                   // 6
        var sumB = (2, 4, 8).Product<TInt, int>();                               // 64
        var flag = ("one", "two", "three").Contains<TString, string>("one");     // true
    }
}
