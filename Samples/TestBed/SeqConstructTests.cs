using System.Diagnostics;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public static class SeqConstructTests
{
    public static void Test()
    {
        Create_Seq_with_constructor();
        Create_Seq_with_collection_expression();
    }

    public static void Create_Seq_with_constructor()
    {
        var seq = new Seq<string>(["a", "b"]);
        Debug.Assert(seq.Count == 2);
    }

    public static void Create_Seq_with_collection_expression()
    {
        Seq<string> seq = ["a", "b"];
        Debug.Assert(seq.Count == 2);
    }
}
