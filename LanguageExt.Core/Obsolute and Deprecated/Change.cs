namespace LanguageExt;

internal class Change
{
    public const string UseEffMonadInstead =
        "This type has been deprecated.  Please use the Eff monad instead.  https://github.com/louthy/language-ext/discussions/1269";

    public const string UseEffMonadInsteadOfAff =
        "Aff has been deprecated.  Please use the Eff monad instead.  https://github.com/louthy/language-ext/discussions/1269";

    public const string UseTransducersInstead =
        "This functionality is now available and has many more features in Transducers.  See the wiki on Transducers to see how to get use transducers";

    public const string UseCollectionIntialiser =
        "Use collection intialiser instead.  So, instead of: (x, y, z), you should now call [x, y, z]";

    public const string UseCollectionIntialiserSeq =
        "Use collection intialiser instead.  So, instead of: Seq1(x), you should now call [x] - alternatively use Seq(x) as Seq1(x) has been deprecated.";
}
