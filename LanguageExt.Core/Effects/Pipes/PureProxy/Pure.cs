
namespace LanguageExt.Pipes
{
    public class Pure<A>
    {
        public readonly A Value;
        public Pure(A value) =>
            Value = value;
    }
}
