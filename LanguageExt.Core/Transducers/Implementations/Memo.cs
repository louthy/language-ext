#nullable enable

using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

record MemoTransducer<EqA, A, B>(Transducer<A, B> Transducer) : Transducer<A, B>
    where EqA : Eq<A>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(Transducer, reduce);

    record Reduce<S>(Transducer<A, B> Transducer, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        readonly AtomHashMap<EqA, A, TResult<S>> memos = AtomHashMap<EqA, A, TResult<S>>(); 

        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            var item = memos.Find(value);
            if (item.IsSome) return (TResult<S>)item;
            var result = Transducer.Transform(Reducer).Run(state, stateValue, value);
            memos.AddOrUpdate(value, result);
            return result;
        }
    }
            
    public override string ToString() =>  
        "memo";
}

record Memo1Transducer<EqA, A, B>(Transducer<A, B> Transducer) : Transducer<A, B>
    where EqA : Eq<A>
{
    readonly AtomHashMap<EqKey, Key, TResultBase> memos = AtomHashMap<EqKey, Key, TResultBase>(); 
    
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(Transducer, reduce, memos);

    record Reduce<S>(Transducer<A, B> Transducer, Reducer<B, S> Reducer, AtomHashMap<EqKey, Key, TResultBase> Memos) 
        : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            var key = new Key(typeof(S), value);
            var item = Memos.Find(key);
            if (item.IsSome) return (TResult<S>)item;
            var result = Transducer.Transform(Reducer).Run(state, stateValue, value);
            Memos.AddOrUpdate(key, result);
            return result;
        }
    }

    record struct Key(Type StateType, A Value);

    struct EqKey : Eq<Key>
    {
        public Task<int> GetHashCodeAsync(Key x) =>
            GetHashCode(x).AsTask();

        public int GetHashCode(Key x) =>
            FNV32.Next(x.StateType.GetHashCode(), default(EqA).GetHashCode(x.Value));
        
        public Task<bool> EqualsAsync(Key x, Key y) =>
            Equals(x, y).AsTask();

        public bool Equals(Key x, Key y) =>
            x.StateType == y.StateType && default(EqA).Equals(x.Value, y.Value);
    }
            
    public override string ToString() =>  
        "memo";
}
