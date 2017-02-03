namespace LanguageExt
{
	using System;

	public static partial class Prelude
	{
		public static Union<T1> Bind<A, T1>(this A a, Func<A, Union<T1>> func) => func(a);

		public static Union<T1, T2> Bind<A, T1, T2>(this A a, Func<A, Union<T1, T2>> func) => func(a);

		public static Union<T1, T2, T3> Bind<A, T1, T2, T3>(this A a, Func<A, Union<T1, T2, T3>> func) => func(a);

		public static Union<T1, T2, T3, T4> Bind<A, T1, T2, T3, T4>(this A a, Func<A, Union<T1, T2, T3, T4>> func) => func(a);

		public static Union<T1, T2, T3, T4, T5> Bind<A, T1, T2, T3, T4, T5>(this A a, Func<A, Union<T1, T2, T3, T4, T5>> func) => func(a);

		public static Union<T1, T2, T3, T4, T5, T6> Bind<A, T1, T2, T3, T4, T5, T6>(this A a, Func<A, Union<T1, T2, T3, T4, T5, T6>> func) => func(a);

		public static Union<T1, T2, T3, T4, T5, T6, T7> Bind<A, T1, T2, T3, T4, T5, T6, T7>(this A a, Func<A, Union<T1, T2, T3, T4, T5, T6, T7>> func) => func(a);

		public static Union<T1, T2, T3, T4, T5, T6, T7, T8> Bind<A, T1, T2, T3, T4, T5, T6, T7, T8>(this A a, Func<A, Union<T1, T2, T3, T4, T5, T6, T7, T8>> func) => func(a);

		public static Union<B> Select<A, B>(this Union<A> source, Func<Union<A>, B> predicate) =>
			source.Bind(aval => new Union<B>(predicate(aval)));

		//public static IUnion<TSource> Where<TSource>(this IUnion<TSource> source, Func<TSource, bool> predicate) =>
		//	source.Match(
		//			(s) => predicate(s) ? source : new Union<TSource>(),
		//			() => new Union<TSource>());

		//public static Option<T> Optional<T>(this Union source) =>
		//	source == null
		//		? new Option<T>()
		//		: source.opt;

		public static TReturn Match<T1, TReturn>(this Union<T1> union, Func<T1, TReturn> f1)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, TReturn>(this Union<T1, T2> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, TReturn>(this Union<T1, T2, T3> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, T4, TReturn>(this Union<T1, T2, T3, T4> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3, Func<T4, TReturn> f4)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));
			unionTuple.Item4.IfSome(tm => retVal = f4(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, T4, T5, TReturn>(this Union<T1, T2, T3, T4, T5> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3, Func<T4, TReturn> f4, Func<T5, TReturn> f5)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));
			unionTuple.Item4.IfSome(tm => retVal = f4(tm));
			unionTuple.Item5.IfSome(tm => retVal = f5(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, T4, T5, T6, TReturn>(this Union<T1, T2, T3, T4, T5, T6> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3, Func<T4, TReturn> f4, Func<T5, TReturn> f5, Func<T6, TReturn> f6)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));
			unionTuple.Item4.IfSome(tm => retVal = f4(tm));
			unionTuple.Item5.IfSome(tm => retVal = f5(tm));
			unionTuple.Item6.IfSome(tm => retVal = f6(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, T4, T5, T6, T7, TReturn>(this Union<T1, T2, T3, T4, T5, T6, T7> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3, Func<T4, TReturn> f4, Func<T5, TReturn> f5, Func<T6, TReturn> f6, Func<T7, TReturn> f7)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));
			unionTuple.Item4.IfSome(tm => retVal = f4(tm));
			unionTuple.Item5.IfSome(tm => retVal = f5(tm));
			unionTuple.Item6.IfSome(tm => retVal = f6(tm));
			unionTuple.Item7.IfSome(tm => retVal = f7(tm));

			return retVal;
		}

		public static TReturn Match<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(this Union<T1, T2, T3, T4, T5, T6, T7, T8> union, Func<T1, TReturn> f1, Func<T2, TReturn> f2, Func<T3, TReturn> f3, Func<T4, TReturn> f4, Func<T5, TReturn> f5, Func<T6, TReturn> f6, Func<T7, TReturn> f7, Func<T8, TReturn> f8)
		{
			var unionTuple = union.ToTuple();
			TReturn retVal = default(TReturn);

			unionTuple.Item1.IfSome(tm => retVal = f1(tm));
			unionTuple.Item2.IfSome(tm => retVal = f2(tm));
			unionTuple.Item3.IfSome(tm => retVal = f3(tm));
			unionTuple.Item4.IfSome(tm => retVal = f4(tm));
			unionTuple.Item5.IfSome(tm => retVal = f5(tm));
			unionTuple.Item6.IfSome(tm => retVal = f6(tm));
			unionTuple.Item7.IfSome(tm => retVal = f7(tm));
			unionTuple.Rest.Item1.IfSome(tm => retVal = f8(tm));

			return retVal;
		}

		public static Func<Func<T1, TReturn>, TReturn> Match<T1, TReturn>(this Union<T1> union)
		{
			var curried = curry((Func<Union<T1>, Func<T1, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, TReturn>> Match<T1, T2, TReturn>(this Union<T1, T2> union)
		{
			var curried = curry((Func<Union<T1, T2>, Func<T1, TReturn>, Func<T2, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, TReturn>>> Match<T1, T2, T3, TReturn>(this Union<T1, T2, T3> union)
		{
			var curried = curry((Func<Union<T1, T2, T3>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, Func<Func<T4, TReturn>, TReturn>>>> Match<T1, T2, T3, T4, TReturn>(this Union<T1, T2, T3, T4> union)
		{
			var curried = curry((Func<Union<T1, T2, T3, T4>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, Func<T4, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, Func<Func<T4, TReturn>, Func<Func<T5, TReturn>, TReturn>>>>> Match<T1, T2, T3, T4, T5, TReturn>(this Union<T1, T2, T3, T4, T5> union)
		{
			var curried = curry((Func<Union<T1, T2, T3, T4, T5>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, Func<T4, TReturn>, Func<T5, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, Func<Func<T4, TReturn>, Func<Func<T5, TReturn>, Func<Func<T6, TReturn>, TReturn>>>>>> Match<T1, T2, T3, T4, T5, T6, TReturn>(this Union<T1, T2, T3, T4, T5, T6> union)
		{
			var curried = curry((Func<Union<T1, T2, T3, T4, T5, T6>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, Func<T4, TReturn>, Func<T5, TReturn>, Func<T6, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, Func<Func<T4, TReturn>, Func<Func<T5, TReturn>, Func<Func<T6, TReturn>, Func<Func<T7, TReturn>, TReturn>>>>>>> Match<T1, T2, T3, T4, T5, T6, T7, TReturn>(this Union<T1, T2, T3, T4, T5, T6, T7> union)
		{
			var curried = curry((Func<Union<T1, T2, T3, T4, T5, T6, T7>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, Func<T4, TReturn>, Func<T5, TReturn>, Func<T6, TReturn>, Func<T7, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Func<Func<T1, TReturn>, Func<Func<T2, TReturn>, Func<Func<T3, TReturn>, Func<Func<T4, TReturn>, Func<Func<T5, TReturn>, Func<Func<T6, TReturn>, Func<Func<T7, TReturn>, Func<Func<T8, TReturn>, TReturn>>>>>>>> Match<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(this Union<T1, T2, T3, T4, T5, T6, T7, T8> union)
		{
			var curried = curry((Func<Union<T1, T2, T3, T4, T5, T6, T7, T8>, Func<T1, TReturn>, Func<T2, TReturn>, Func<T3, TReturn>, Func<T4, TReturn>, Func<T5, TReturn>, Func<T6, TReturn>, Func<T7, TReturn>, Func<T8, TReturn>, TReturn>)Match);
			return curried(union);
		}

		public static Action<Action<T1>> Match<T1>(this Union<T1> union)
		{
			var unionTuple = union.ToTuple();
			return act((Action<T1> a) => unionTuple.Item1.IfSome(a));
		}

		public static Func<Action<T1>, Action<Action<T2>>> Match<T1, T2>(this Union<T1, T2> union)
		{
			var unionTuple = union.ToTuple();
			var bAct = act((Action<T2> b) => unionTuple.Item2.IfSome(b));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Action<Action<T3>>>> Match<T1, T2, T3>(this Union<T1, T2, T3> union)
		{
			var unionTuple = union.ToTuple();
			var cAct = act((Action<T3> c) => unionTuple.Item3.IfSome(c));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Func<Action<T3>, Action<Action<T4>>>>> Match<T1, T2, T3, T4>(this Union<T1, T2, T3, T4> union)
		{
			var unionTuple = union.ToTuple();
			var dAct = act((Action<T4> d) => unionTuple.Item4.IfSome(d));
			var cAct = fun((Action<T3> c) => unionTuple.Item3.IfSome(c).Return(dAct));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Func<Action<T3>, Func<Action<T4>, Action<Action<T5>>>>>> Match<T1, T2, T3, T4, T5>(this Union<T1, T2, T3, T4, T5> union)
		{
			var unionTuple = union.ToTuple();
			var eAct = act((Action<T5> e) => unionTuple.Item5.IfSome(e));
			var dAct = fun((Action<T4> d) => unionTuple.Item4.IfSome(d).Return(eAct));
			var cAct = fun((Action<T3> c) => unionTuple.Item3.IfSome(c).Return(dAct));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Func<Action<T3>, Func<Action<T4>, Func<Action<T5>, Action<Action<T6>>>>>>> Match<T1, T2, T3, T4, T5, T6>(this Union<T1, T2, T3, T4, T5, T6> union)
		{
			var unionTuple = union.ToTuple();
			var fAct = act((Action<T6> f) => unionTuple.Item6.IfSome(f));
			var eAct = fun((Action<T5> e) => unionTuple.Item5.IfSome(e).Return(fAct));
			var dAct = fun((Action<T4> d) => unionTuple.Item4.IfSome(d).Return(eAct));
			var cAct = fun((Action<T3> c) => unionTuple.Item3.IfSome(c).Return(dAct));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Func<Action<T3>, Func<Action<T4>, Func<Action<T5>, Func<Action<T6>, Action<Action<T7>>>>>>>> Match<T1, T2, T3, T4, T5, T6, T7>(this Union<T1, T2, T3, T4, T5, T6, T7> union)
		{
			var unionTuple = union.ToTuple();
			var gAct = act((Action<T7> g) => unionTuple.Item7.IfSome(g));
			var fAct = fun((Action<T6> f) => unionTuple.Item6.IfSome(f).Return(gAct));
			var eAct = fun((Action<T5> e) => unionTuple.Item5.IfSome(e).Return(fAct));
			var dAct = fun((Action<T4> d) => unionTuple.Item4.IfSome(d).Return(eAct));
			var cAct = fun((Action<T3> c) => unionTuple.Item3.IfSome(c).Return(dAct));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Func<Action<T1>, Func<Action<T2>, Func<Action<T3>, Func<Action<T4>, Func<Action<T5>, Func<Action<T6>, Func<Action<T7>, Action<Action<T8>>>>>>>>> Match<T1, T2, T3, T4, T5, T6, T7, T8>(this Union<T1, T2, T3, T4, T5, T6, T7, T8> union)
		{
			var unionTuple = union.ToTuple();
			var hAct = act((Action<T8> h) => unionTuple.Rest.Item1.IfSome(h));
			var gAct = fun((Action<T7> g) => unionTuple.Item7.IfSome(g).Return(hAct));
			var fAct = fun((Action<T6> f) => unionTuple.Item6.IfSome(f).Return(gAct));
			var eAct = fun((Action<T5> e) => unionTuple.Item5.IfSome(e).Return(fAct));
			var dAct = fun((Action<T4> d) => unionTuple.Item4.IfSome(d).Return(eAct));
			var cAct = fun((Action<T3> c) => unionTuple.Item3.IfSome(c).Return(dAct));
			var bAct = fun((Action<T2> b) => unionTuple.Item2.IfSome(b).Return(cAct));
			return fun((Action<T1> a) => unionTuple.Item1.IfSome(a).Return(bAct));
		}

		public static Tuple<Option<T1>> ToTuple<T1>(this Union<T1> union) =>
			((IUnion<T1>)union).InternalValue() ?? System.Tuple.Create(Option<T1>.None);

		public static Tuple<Option<T1>, Option<T2>> ToTuple<T1, T2>(this Union<T1, T2> union) =>
			((IUnion<T1, T2>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>> ToTuple<T1, T2, T3>(this Union<T1, T2, T3> union) =>
			((IUnion<T1, T2, T3>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>> ToTuple<T1, T2, T3, T4>(this Union<T1, T2, T3, T4> union) =>
			((IUnion<T1, T2, T3, T4>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None,
				Option<T4>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>> ToTuple<T1, T2, T3, T4, T5>(this Union<T1, T2, T3, T4, T5> union) =>
			((IUnion<T1, T2, T3, T4, T5>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None,
				Option<T4>.None,
				Option<T5>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>
			ToTuple<T1, T2, T3, T4, T5, T6>(this Union<T1, T2, T3, T4, T5, T6> union) =>
			((IUnion<T1, T2, T3, T4, T5, T6>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None,
				Option<T4>.None,
				Option<T5>.None,
				Option<T6>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>
			ToTuple<T1, T2, T3, T4, T5, T6, T7>(this Union<T1, T2, T3, T4, T5, T6, T7> union) =>
			((IUnion<T1, T2, T3, T4, T5, T6, T7>)union).InternalValue() ?? Tuple(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None,
				Option<T4>.None,
				Option<T5>.None,
				Option<T6>.None,
				Option<T7>.None);

		public static Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Tuple<Option<T8>>>
			ToTuple<T1, T2, T3, T4, T5, T6, T7, T8>(this Union<T1, T2, T3, T4, T5, T6, T7, T8> union) =>
			  ((IUnion<T1, T2, T3, T4, T5, T6, T7, T8>)union).InternalValue() ?? System.Tuple.Create(
				Option<T1>.None,
				Option<T2>.None,
				Option<T3>.None,
				Option<T4>.None,
				Option<T5>.None,
				Option<T6>.None,
				Option<T7>.None,
				Option<T8>.None);

		public static Union<T, Err> ToErrorUnion<T, Err>(this Func<T> factory)
			where Err : SystemException
		{
			try
			{
				return new Union<T, Err>(factory());
			}
			catch (Err ex)
			{
				return new Union<T, Err>(ex);
			}
		}

		public static Union<T, Err1, Err2> ToErrorUnion<T, Err1, Err2>(this Func<T> factory)
		where Err1 : SystemException
		where Err2 : SystemException
		{
			try
			{
				return new Union<T, Err1, Err2>(factory());
			}
			catch (Err1 ex)
			{
				return new Union<T, Err1, Err2>(ex);
			}
			catch (Err2 ex)
			{
				return new Union<T, Err1, Err2>(ex);
			}
		}

		public static Union<T, SystemException> ToErrorUnion<T>(this Func<T> factory)
		{
			try
			{
				return new Union<T, SystemException>(factory());
			}
			catch (SystemException ex)
			{
				return new Union<T, SystemException>(ex);
			}
		}
	}
}