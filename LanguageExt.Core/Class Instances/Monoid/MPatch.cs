using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Monoid instance for Patch
    /// </summary>
    public struct MPatch<EqA, A> : Monoid<Patch<EqA, A>>, Eq<Patch<EqA, A>> where EqA : struct, Eq<A>
    {
        /// <summary>
        /// Monoid append: produces a patch is a merged version of both provided
        /// patches.  
        /// </summary>
        public Patch<EqA, A> Append(Patch<EqA, A> px, Patch<EqA, A> py)
        {
            Seq<Edit<EqA, A>> replace(int i, A o, A n, Seq<Edit<EqA, A>> seq) =>
                default(EqA).Equals(o, n)
                    ? seq
                    : Edit<EqA, A>.Replace.New(i, o, n).Cons(seq);

            Seq<Edit<EqA, A>> merge(Seq<Edit<EqA, A>> ex, Seq<Edit<EqA, A>> ey, int off)
            {
                if (ex.IsEmpty)
                {
                    return ey.Map(e => e.Index(i => i + off));
                }
                else if (ey.IsEmpty)
                {
                    return ex;
                }
                else
                {
                    var x = ex.Head;
                    var xs = ex.Tail;
                    var y = ey.Head;
                    var ys = ey.Tail;

                    var yi = y.Index(i => i + off);
                    var ord = x.Position.CompareTo(yi.Position);
                    if (ord < 0)
                    {
                        return x.Cons(merge(xs, ey, off + offset(x)));
                    }
                    else if (ord > 0)
                    {
                        return yi.Cons(merge(ex, ys, off));
                    }
                    else
                    {
                        if (x is Edit<EqA, A>.Delete del1 && yi is Edit<EqA, A>.Insert ins1)
                        {
                            return replace(del1.Position, del1.Element, ins1.Element, merge(xs, ys, off + offset(x)));
                        }
                        else if (x is Edit<EqA, A>.Delete del2)
                        {
                            return x.Cons(merge(xs, ey, off + offset(x)));
                        }
                        else if (yi is Edit<EqA, A>.Insert ins2)
                        {
                            return yi.Cons(merge(ex, ys, off));
                        }
                        else if (x is Edit<EqA, A>.Replace replA1 && yi is Edit<EqA, A>.Replace replB1)
                        {
                            return replace(replA1.Position, replA1.Element, replB1.ReplaceElement, merge(xs, ys, off));
                        }
                        else if (x is Edit<EqA, A>.Replace replA2 && yi is Edit<EqA, A>.Delete del3)
                        {
                            return Edit<EqA, A>.Delete.New(replA2.Position, replA2.Element).Cons(merge(xs, ys, off));
                        }
                        else if (x is Edit<EqA, A>.Insert ins3 && yi is Edit<EqA, A>.Replace replB2)
                        {
                            return Edit<EqA, A>.Insert.New(ins3.Position, replB2.ReplaceElement).Cons(merge(xs, ys, off + offset(x)));
                        }
                        else if (x is Edit<EqA, A>.Insert ins4 && yi is Edit<EqA, A>.Delete del4)
                        {
                            return merge(xs, ys, off + offset(x));
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }
            }

            int offset(Edit<EqA, A> edit) =>
                edit is Edit<EqA, A>.Insert ? -1
              : edit is Edit<EqA, A>.Delete ? 1
              : edit is Edit<EqA, A>.Replace ? 0
              : throw new NotSupportedException();

            return new Patch<EqA, A>(merge(px.Edits, py.Edits, 0));
        }

        /// <summary>
        /// Monoid empty value: A patch with no edits
        /// </summary>
        public Patch<EqA, A> Empty() =>
            Patch<EqA, A>.Empty;

        public bool Equals(Patch<EqA, A> x, Patch<EqA, A> y) =>
            default(EqPatch<EqA, A>).Equals(x, y);

        public int GetHashCode(Patch<EqA, A> x) =>
            default(EqPatch<EqA, A>).GetHashCode(x);
 
        [Pure]
        public Task<bool> EqualsAsync(Patch<EqA, A> x, Patch<EqA, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Patch<EqA, A> x) =>
            GetHashCode(x).AsTask();    
    }
}
