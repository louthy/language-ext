namespace LanguageExt.Traits;

public interface Indexable<in A, in KEY, VALUE>
{
    public static abstract Option<VALUE> TryGet(A ma, KEY key);
    public static abstract VALUE Get(A ma, KEY key);
}
