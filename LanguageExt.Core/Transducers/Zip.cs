using System.Threading.Tasks;

namespace LanguageExt.Transducers;

/*
    // TODO
    
record ZipTransducer2<E, A, B>(Transducer<E, A> First, Transducer<E, A> Second)
    : Transducer<E, (A First, B Second)>
{
    public Transducer<E, (A First, B Second)> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, (A First, B Second)> reduce)
    {
    }
}
*/
