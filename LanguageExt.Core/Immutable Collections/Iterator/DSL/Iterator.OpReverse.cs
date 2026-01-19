using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpReverse(Iterator<A> iter) : Iterator<A>
    {
        public override string ToString() => 
            $"Reverse({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            return go().Next();
            Iterator<A> go()
            {
                var writer = ArrayWriter<A>.Init();

                for (var i = iter; i is (Exist<A> (var head), var tail); i = tail)
                {
                    writer.Add(head);
                }

                var (array, start, count) = writer.ToArrayBack();
                var arr = new Arr<A>(array, start, count);
                return Iterator.forward(arr);
            }
        }
    }
}
