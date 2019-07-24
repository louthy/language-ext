using System;

namespace AccountingDSL.DSL
{
    public static class TransformLINQ
    {
        public static Transform<B> Bind<A, B>(this Transform<A> ma, Func<A, Transform<B>> f)
        {
            switch (ma)
            {
                case Transform<A>.Return item: return f(item.Value);
                case Transform<A>.Fail item: return new Transform<B>.Fail(item.Value);
                case Transform<A>.Log item: return new Transform<B>.Log(item.Value, n => item.Next(n).Bind(f));
                case Transform<A>.Invoke item: return new Transform<B>.Invoke(item.Func, item.Args, n => item.Next(n).Bind(f));
                case Transform<A>.SetValue item: return new Transform<B>.SetValue(item.Name, item.Value, n => item.Next(n).Bind(f));
                case Transform<A>.GetValue item: return new Transform<B>.GetValue(item.Name, n => item.Next(n).Bind(f));
                case Transform<A>.Compute item: return new Transform<B>.Compute(item.Operation, item.SourceType, n => item.Next(n).Bind(f));
                case Transform<A>.Print item: return new Transform<B>.Print(item.Operation, item.Messages, n => item.Next(n).Bind(f));
                case Transform<A>.AllRows item: return new Transform<B>.AllRows(n => item.Next(n).Bind(f));
                case Transform<A>.FilterRows item: return new Transform<B>.FilterRows(item.Value, n => item.Next(n).Bind(f));
                default: throw new NotImplementedException();
            }
        }

        public static Transform<B> Map<A, B>(this Transform<A> ma, Func<A, B> f) =>
            ma.Bind(x => Transform.Return(f(x)));

        public static Transform<B> Select<A, B>(this Transform<A> ma, Func<A, B> f) =>
            ma.Bind(x => Transform.Return(f(x)));

        public static Transform<B> SelectMany<A, B>(this Transform<A> ma, Func<A, Transform<B>> f) =>
            ma.Bind(f);

        public static Transform<C> SelectMany<A, B, C>(this Transform<A> ma, Func<A, Transform<B>> bind, Func<A, B, C> project) =>
            ma.Bind(a => bind(a).Map(b => project(a, b)));
    }
}
