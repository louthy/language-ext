namespace LanguageExt;

public partial class IteratorIO
{
    internal sealed class OpZip<A, B>(IteratorIO<A> xs, IteratorIO<B> ys) : IteratorIO<(A First, B Second)>
    {
        public override string ToString() => 
            $"Zip({xs}, {ys})";
        
        public override IO<(Head<(A First, B Second)> Head, IteratorIO<(A First, B Second)> Tail)> NextIO() =>
            (((Head<A> Head, IteratorIO<A> Tail) x, (Head<B> Head, IteratorIO<B> Tail) y) =>
                 (x, y) switch
                 {
                     ((Exist<A> (var lh), var lt), (Exist<B> (var rh), var rt)) =>
                         (new Exist<(A First, B Second)>((lh, rh)), lt.Zip(rt)),

                     _ => Head.NilIO<(A First, B Second)>()
                 })
              * xs.NextIO()
              * ys.NextIO();        

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override IteratorIO<(A First, B Second)> Using() =>
            new OpZip<A, B>(xs.Using(), ys.Using());
    }
}
