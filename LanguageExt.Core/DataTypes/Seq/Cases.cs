
namespace LanguageExt
{
    public interface SeqCase<A>
    {
    }

    public sealed class EmptyCase<A> : SeqCase<A>
    {
        internal static SeqCase<A> Default = new EmptyCase<A>();
        internal EmptyCase() { }
    }

    public sealed class HeadCase<A> : SeqCase<A>
    {
        public readonly A Head;
        internal HeadCase(A head) => Head = head;
        public void Deconstruct(out A Head) => Head = this.Head;
        internal static SeqCase<A> New(A head) => new HeadCase<A>(head);
    }

    public sealed class HeadTailCase<A> : SeqCase<A>
    {
        public readonly A Head;
        
        public readonly Seq<A> Tail;
        
        internal HeadTailCase(A head, Seq<A> tail)
        {
            Head = head;
            Tail = tail;
        }
        
        public void Deconstruct(out A Head, out Seq<A> Tail)
        {
            Head = this.Head;
            Tail = this.Tail;
        }
        
        internal static SeqCase<A> New(A head, Seq<A> tail) => 
            new HeadTailCase<A>(head, tail);
    }
}
