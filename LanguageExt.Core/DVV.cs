//
// TODO: Work in progress - Dotted Version Vectors
//

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using LanguageExt;
//using static LanguageExt.Prelude;

//namespace LanguageExt
//{
//    /// <summary>
//    /// Dotted version vector
//    /// </summary>
//    public class DVV : IEquatable<DVV>
//    {
//        public readonly DotVector Vector;
//        public readonly Dot Dot;

//        public readonly static DVV Zero = new DVV();

//        internal DVV()
//        {
//        }

//        internal DVV(DotVector vector, Dot dot)
//        {
//            Vector = vector;
//            Dot = dot;
//        }

//        public bool Equals(DVV other) =>
//            !ReferenceEquals(other,null) && 
//             Tuple(Vector, Dot) == Tuple(other.Vector, other.Dot);
//    }

//    public class DotVector : IEquatable<DotVector>
//    {
//        public readonly Lst<Dot> Dots;

//        public DotVector(Lst<Dot> dots)
//        {
//            Dots = dots;
//        }

//        public static readonly DotVector Empty = new DotVector(List.empty<Dot>());

//        public bool Equals(DotVector other) =>
//            !ReferenceEquals(other, null) && 
//             Dots == other.Dots;
//    }

//    public class Dot : IEquatable<Dot>
//    {
//        public readonly int Id;
//        public readonly long Counter;
//        public readonly long Timestamp;

//        public Dot(int id, long counter, long timestamp)
//        {
//            Id = id;
//            Counter = counter;
//            Timestamp = timestamp;
//        }

//        public bool Equals(Dot other) =>
//            !ReferenceEquals(other, null) &&
//            Id == other.Id &&
//            Counter == other.Counter;
//    }

//    public static class DVVModule
//    {
//        /// <summary>
//        /// Returns a clock that newer that the client and server clocks at Id.
//        /// </summary>
//        /// <param name="sc">Set of clocks from the Client</param>
//        /// <param name="sr">Set of clocks from the Replica</param>
//        /// <param name="id">id that will be incremented (Replica id)</param>
//        /// <returns></returns>
//        public static DVV update(
//            Lst<DVV> sc,
//            Lst<DVV> sr,
//            int id
//            )
//        {
//            if( sr.Count == 1 && sr[1] == DVV.Zero)
//            {
//                return update2(merge(sc), DVV.Zero, id);
//            }
//            return update2(merge(sc), sr, id);
//        }

//        static DVV update2(
//            DVV sc,
//            DVV sr,
//            int id
//            )
//        {
//            if(sc == DVV.Zero && sr == DVV.Zero)
//            {
//                return new DVV(DotVector.Empty, new Dot(id, 1, Environment.TickCount));
//            }
//            if (sc == DVV.Zero)
//            {
//                var res2 = maxCounter(id, sr); // Item1 == Max
//                var dot2 = new Dot(id, res2.Item1 + 1, Environment.TickCount);
//                return new DVV(DotVector.Empty, dot2);
//            }

//            var res = maxCounter(id, sr); // Item1 == Max
//            var dot = new Dot(id, res.Item1 + 1, Environment.TickCount);
//            return new DVV(sc.Vector, dot);
//        }

//        /// <summary>
//        /// Increment DottedVV at Node.
//        /// </summary>
//        public static DVV increment(int id, DVV c) =>
//            update(List(c), List(c), id);

//        /// <summary>
//        /// Return true if Va is a direct descendant of Vb, else false --> S1 >= S2
//        /// </summary>
//        public static bool descends(DVV a, DVV b) =>
//            equal(a, b) || strictDescends(a, b);

//        /// <summary>
//        /// Return true if Va is a direct descendant of Vb, else false --> S1 > S2
//        /// </summary>
//        public static bool strictDescends(DVV a, DVV b)
//        {
//            if (a == DVV.Zero) return false;
//            if (a.Vector.Dots.Count == 0) return false;
//            if (b == DVV.Zero) return true;
//            if (b.Vector.Dots.Count == 0) return true;
//            return !equal(a, b) && descends2(a, b);
//        }

//        static bool descends2(DVV a, DVV b)
//        {
//            // descends2({V,{I,{C,TA}}}, {V,{I,{C,TB}}}) -> (TA > TB);
//            if (notnull(a.Dot) && notnull(b.Dot) &&
//               a.Vector == b.Vector &&
//               a.Dot.Id == b.Dot.Id &&
//               a.Dot.Counter == b.Dot.Counter)
//            {
//                return a.Dot.Timestamp > b.Dot.Timestamp;
//            }

//            // descends2({VA,_DA}, {_VB,{IB,{CB,_TB}}}) -> 
//            //  case lists: keyfind(IB, 1, VA) of
//            //      { _, { CA, _TA} } ->     
//            //           (CA >= CB); % orelse((CA =:= CB) and(TA > TB));
//            //      false-> false %% they are not equal, as it was tested in strict_descends
//            //  end;
//            if (notnull(b.Dot))
//            {
//                var va = a.Vector.Dots;
//                var ib = b.Dot.Id;
//                var cb = b.Dot.Counter;

//                return va.Count > 0 && va[0].Counter >= cb;
//            }

//            // descends2(A, B) ->
//            //     {VA, null} = merge(A), 
//            //     { VB, null} = merge(B), 
//            //      descends3(VA, VB).
//            var mva = merge(a).Vector;
//            var mvb = merge(b).Vector;
//            return descends3(mva.Dots, mvb.Dots);
//        }

//        static bool descends3(IEnumerable<Dot> va, IEnumerable<Dot> vb)
//        {
//            if (va == vb) return true;
//            if (!vb.Any()) return true;

//            var vbHead = vb.Head();
//            var tl = vb.Tail();
//            var nodeB = vbHead.Id;
//            var ctrB = vbHead.Counter;
//            var vaHead = va.Head();

//            if (vaHead.Id == nodeB )
//            {
//                return vaHead.Counter >= ctrB && descends3(va, tl);
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public static DVV merge(DVV dvv)
//        {
//            if (dvv == DVV.Zero) return DVV.Zero;
//            return mergeDot(dvv);
//        }

//        public static DVV merge(Lst<DVV> dvvs)
//        {
//            if (!dvvs.Any()) return DVV.Zero;
//            var head = dvvs.Head();
//            var tail = dvvs.Tail();
//            var anyTail = tail.Any();
//            if (head == DVV.Zero && anyTail) return merge(tail.Freeze());
//            if (head == DVV.Zero && !anyTail) return DVV.Zero;
//            if (!anyTail) return mergeDot(head);

//            return merge(tail.Freeze(), sortAndMergeDot(head));
//        }

//        static DVV merge(Lst<DVV> tail, Lst<Dot> nclock)
//        {
//            var any = tail.Any();
//            if (!any) return new DVV(new DotVector(nclock), null);

//            var aclock = tail.Head();
//            var vclocks = tail.Tail();
//            return merge(vclocks.Freeze(), merge(sortAndMergeDot(aclock), nclock, List.empty<Dot>()));
//        }

//        static Lst<Dot> merge(Lst<Dot> left, Lst<Dot> right, Lst<Dot> accClock )
//        {
//            if (left.Count == 0 && right.Count == 0) return accClock.Reverse().Freeze();
//            if (left.Count == 0) return accClock.Reverse().Append(right).Freeze();
//            if (right.Count == 0) return accClock.Reverse().Append(left).Freeze();

//            var v = left;
//            var n = right;

//            var nct1 = v.Head();
//            var vclock = v.Tail().Freeze();

//            var nct2 = n.Head();
//            var nclock = n.Tail().Freeze();

//            if (nct1.Id < nct2.Id)
//            {
//                return merge(vclock, n, nct1.Cons(accClock));
//            }
//            else if( nct1.Id > nct2.Id)
//            {
//                return merge(v, nclock, nct2.Cons(accClock));
//            }
//            else
//            {
//                var ct = nct1.Counter > nct2.Counter ? Tuple(nct1.Counter, nct1.Timestamp)
//                       : nct1.Counter < nct2.Counter ? Tuple(nct2.Counter, nct2.Timestamp)
//                       : Tuple(nct1.Counter, Math.Max(nct1.Timestamp, nct2.Timestamp));

//                return merge(vclock, nclock, new Dot(nct1.Id, ct.Item1, ct.Item2).Cons(accClock));
//            }

//        }

//        static Lst<Dot> sortAndMergeDot(DVV dvv)
//        {
//            var s2 = mergeDot(dvv);
//            return s2.Vector.Dots.OrderBy(x => x.Id).Freeze();
//        }

//        static DVV mergeDot(DVV dvv)
//        {
//            // merge_dot({S, null}) -> {S, null};
//            if (dvv.Dot == null) return new DVV(dvv.Vector, null);

//            // merge_dot({S, {Id, C}}) -> {lists:keystore(Id, 1, S, {Id, C}), null}.
//            if ( dvv.Vector.Dots.Count > 0 &&
//                 dvv.Vector.Dots[0].Id == dvv.Dot.Id )
//            {
//                var vec = new DotVector(dvv.Vector.Dots.SetItem(0, dvv.Dot));
//                return new DVV(vec, null);
//            }
//            else
//            {
//                var vec = new DotVector(dvv.Vector.Dots.Add(dvv.Dot));
//                return new DVV(vec, null);
//            }
//        }
//    }
//}
