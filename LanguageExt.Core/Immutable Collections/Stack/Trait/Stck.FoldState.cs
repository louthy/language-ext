namespace LanguageExt;

public partial class Stck
{
    public ref struct FoldState
    {
        object Top;

        internal static FoldState Setup<A>(Stck<A> top)
        {
            FoldState state = default;
            state.Top = top;
            return state;
        }

        internal static bool Step<A>(ref FoldState state, out A value)
        {
            ref var top = ref state.Top;
            switch (top)
            {
                case Stck<A>.Top(var t, var r):
                    top = r;
                    value = t;
                    return true;
                
                default:
                    value = default!;
                    return false;
            }
        }
    }
}
