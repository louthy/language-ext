namespace LanguageExt;

internal class Change
{
    public const int Priority = int.MinValue;
    
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
    
    public const string NullableMessage =
        "Before Seq was an actual data-type, it was a conversion function from either 'potentially nullable things' " +
        "or 'types that may or may not yield a value' into a sequence. With the advent of nullable references " +
        "and better pattern matching features, this is now deprecated.  Please use a null/no-value check instead.";


}
