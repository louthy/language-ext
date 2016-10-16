using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
	public interface IUnion
	{
	}

	public interface IUnion<T1>
	{
		Tuple<Option<T1>> InternalValue();
	}

	public interface IUnion<T1, T2> : IUnion
	{
		Tuple<Option<T1>, Option<T2>> InternalValue();
	}

	public interface IUnion<T1, T2, T3> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>> InternalValue();
	}

	public interface IUnion<T1, T2, T3, T4> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>> InternalValue();
	}

	public interface IUnion<T1, T2, T3, T4, T5> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>> InternalValue();
	}

	public interface IUnion<T1, T2, T3, T4, T5, T6> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>> InternalValue();
	}

	public interface IUnion<T1, T2, T3, T4, T5, T6, T7> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>> InternalValue();
	}

	public interface IUnion<T1, T2, T3, T4, T5, T6, T7, T8> : IUnion
	{
		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Tuple<Option<T8>>> InternalValue();
	}

	public class Union<T1> : IUnion, IUnion<T1>
	{
		private readonly Tuple<Option<T1>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = System.Tuple.Create(Some(value));
		}

		Tuple<Option<T1>> IUnion<T1>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None);
		}

		public static implicit operator Union<T1>(T1 item)
		{
			return new Union<T1>(item);
		}
	}

	public class Union<T1, T2> : IUnion, IUnion<T1, T2>
	{
		private readonly Tuple<Option<T1>, Option<T2>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>>(value, Option<T2>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>>(Option<T1>.None, value);
		}

		Tuple<Option<T1>, Option<T2>> IUnion<T1, T2>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None);
		}

		public static implicit operator Union<T1, T2>(T1 item)
		{
			return new Union<T1, T2>(item);
		}

		public static implicit operator Union<T1, T2>(T2 item)
		{
			return new Union<T1, T2>(item);
		}
	}

	public class Union<T1, T2, T3> : IUnion, IUnion<T1, T2, T3>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>>(value, Option<T2>.None, Option<T3>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>>(Option<T1>.None, value, Option<T3>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>>(Option<T1>.None, Option<T2>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>> IUnion<T1, T2, T3>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None);
		}

		public static implicit operator Union<T1, T2, T3>(T1 item)
		{
			return new Union<T1, T2, T3>(item);
		}

		public static implicit operator Union<T1, T2, T3>(T2 item)
		{
			return new Union<T1, T2, T3>(item);
		}

		public static implicit operator Union<T1, T2, T3>(T3 item)
		{
			return new Union<T1, T2, T3>(item);
		}
	}

	public class Union<T1, T2, T3, T4> : IUnion, IUnion<T1, T2, T3, T4>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>>(value, Option<T2>.None, Option<T3>.None, Option<T4>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>>(Option<T1>.None, value, Option<T3>.None, Option<T4>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>>(Option<T1>.None, Option<T2>.None, value, Option<T4>.None);
		}

		public Union(T4 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>> IUnion<T1, T2, T3, T4>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T1 item)
		{
			return new Union<T1, T2, T3, T4>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T2 item)
		{
			return new Union<T1, T2, T3, T4>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T3 item)
		{
			return new Union<T1, T2, T3, T4>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T4 item)
		{
			return new Union<T1, T2, T3, T4>(item);
		}
	}

	public class Union<T1, T2, T3, T4, T5> : IUnion, IUnion<T1, T2, T3, T4, T5>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>>(value, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>>(Option<T1>.None, value, Option<T3>.None, Option<T4>.None, Option<T5>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>>(Option<T1>.None, Option<T2>.None, value, Option<T4>.None, Option<T5>.None);
		}

		public Union(T4 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, value, Option<T5>.None);
		}

		public Union(T5 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>> IUnion<T1, T2, T3, T4, T5>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T1 item)
		{
			return new Union<T1, T2, T3, T4, T5>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T2 item)
		{
			return new Union<T1, T2, T3, T4, T5>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T3 item)
		{
			return new Union<T1, T2, T3, T4, T5>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T4 item)
		{
			return new Union<T1, T2, T3, T4, T5>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T5 item)
		{
			return new Union<T1, T2, T3, T4, T5>(item);
		}
	}

	public class Union<T1, T2, T3, T4, T5, T6> : IUnion, IUnion<T1, T2, T3, T4, T5, T6>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(value, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(Option<T1>.None, value, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(Option<T1>.None, Option<T2>.None, value, Option<T4>.None, Option<T5>.None, Option<T6>.None);
		}

		public Union(T4 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, value, Option<T5>.None, Option<T6>.None);
		}

		public Union(T5 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, value, Option<T6>.None);
		}

		public Union(T6 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>> IUnion<T1, T2, T3, T4, T5, T6>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6>(item);
		}
	}

	public class Union<T1, T2, T3, T4, T5, T6, T7> : IUnion, IUnion<T1, T2, T3, T4, T5, T6, T7>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(value, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, value, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, Option<T2>.None, value, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None);
		}

		public Union(T4 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, value, Option<T5>.None, Option<T6>.None, Option<T7>.None);
		}

		public Union(T5 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, value, Option<T6>.None, Option<T7>.None);
		}

		public Union(T6 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, value, Option<T7>.None);
		}

		public Union(T7 value)
		{
			this.internalValue = Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>> IUnion<T1, T2, T3, T4, T5, T6, T7>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T1 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T2 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T3 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T4 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T5 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T6 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T7 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>(item);
		}
	}

	public class Union<T1, T2, T3, T4, T5, T6, T7, T8> : IUnion, IUnion<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		private readonly Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Tuple<Option<T8>>> internalValue;

		public Union(T1 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(value, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public Union(T2 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, value, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public Union(T3 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, value, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public Union(T4 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, value, Option<T5>.None, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public Union(T5 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, value, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public Union(T6 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, value, Option<T7>.None, Option<T8>.None);
		}

		public Union(T7 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, value, Option<T8>.None);
		}

		public Union(T8 value)
		{
			this.internalValue = System.Tuple.Create<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Option<T8>>(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None, value);
		}

		Tuple<Option<T1>, Option<T2>, Option<T3>, Option<T4>, Option<T5>, Option<T6>, Option<T7>, Tuple<Option<T8>>> IUnion<T1, T2, T3, T4, T5, T6, T7, T8>.InternalValue()
		{
			return internalValue ?? System.Tuple.Create(Option<T1>.None, Option<T2>.None, Option<T3>.None, Option<T4>.None, Option<T5>.None, Option<T6>.None, Option<T7>.None, Option<T8>.None);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T2 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T3 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T4 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T5 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T6 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T7 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T8 item)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>(item);
		}
	}
}