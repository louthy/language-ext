namespace LanguageExt;

public partial class Iterator
{
    internal sealed class OpZip<A, B>(Iterator<A> xs, Iterator<B> ys) : Iterator<(A First, B Second)>
    {
        public override string ToString() => 
            $"Zip({xs}, {ys})";

        public override (Head<(A First, B Second)> Head, Iterator<(A First, B Second)> Tail) Next() =>
            (xs, ys) switch
            {
                ((Exist<A> (var lh), var lt), (Exist<B> (var rh), var rt)) =>
                    (new Exist<(A First, B Second)>((lh, rh)), lt.Zip(rt)),

                _ => (Nil<(A, B)>.Default, Nil.Default)
            };

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override Iterator<(A First, B Second)> Using() =>
            new OpZip<A, B>(xs.Using(), ys.Using());
    }
}
