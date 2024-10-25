namespace LanguageExt;

internal class Change
{
    public const string UseCollectionIntialiser =
        "Use collection intialiser instead.  So, instead of: (x, y, z), you should now call [x, y, z]";

    public const string UseCollectionIntialiserSeq =
        "Use collection intialiser instead.  So, instead of: Seq1(x), you should now call [x] - alternatively use Seq(x) as Seq1(x) has been deprecated.";

    public const string UseToArrayInstead =
        "Use ToArray() instead";

    public const string UseToListInstead =
        "Use ToList() instead";

    public const string UseToSeqInstead =
        "Use ToList() instead";
}
