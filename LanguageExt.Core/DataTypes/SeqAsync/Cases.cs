//using System.Threading.Tasks;

//namespace LanguageExt
//{
//    public interface SeqAsyncCase<A>
//    {
//    }

//    public sealed class EmptyCaseAsync<A> : SeqAsyncCase<A>
//    {
//        internal static SeqAsyncCase<A> Default = new EmptyCaseAsync<A>();
//        internal EmptyCaseAsync() { }
//    }

//    public sealed class HeadCaseAsync<A> : SeqAsyncCase<A>
//    {
//        public readonly Task<A> Head;
//        internal HeadCaseAsync(Task<A> head) => Head = head;
//        public void Deconstruct(out Task<A> Head) => Head = this.Head;
//        internal static SeqAsyncCase<A> New(Task<A> head) => new HeadCaseAsync<A>(head);
//    }

//    public sealed class HeadTailCaseAsync<A> : SeqAsyncCase<A>
//    {
//        public readonly Task<A> Head;
//        public readonly SeqAsync<A> Tail;
        
//        internal HeadTailCaseAsync(Task<A> head, SeqAsync<A> tail)
//        {
//            Head = head;
//            Tail = tail;
//        }
        
//        public void Deconstruct(out Task<A> Head, out SeqAsync<A> Tail)
//        {
//            Head = this.Head;
//            Tail = this.Tail;
//        }
        
//        internal static SeqAsyncCase<A> New(Task<A> head, SeqAsync<A> tail) => 
//            new HeadTailCaseAsync<A>(head, tail);
//    }
//}
