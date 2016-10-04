using System;

namespace LanguageExt
{
    public interface IUnion { }

    public interface IUnion<T>
    {
        Option<T> opt { get; }
    }

    public class Union<T1> : IUnion, IUnion<T1>
    {
        private readonly Option<T1> internalValue;

        public Union()
        {
        }

        public Union(T1 value)
        {
            this.internalValue = value;
        }

        Option<T1> IUnion<T1>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2> : Union<T1>, IUnion, IUnion<T2>
    {
        private readonly Option<T2> internalValue;

        protected Union()
        { }

        public Union(T1 a) : base(a)
        {
        }

        public Union(T2 value)
        {
            this.internalValue = value;
        }

        Option<T2> IUnion<T2>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3> : Union<T1, T2>, IUnion, IUnion<T3>
    {
        private readonly Option<T3> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value)
        {
            this.internalValue = value;
        }

        Option<T3> IUnion<T3>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3, T4> : Union<T1, T2, T3>, IUnion, IUnion<T4>
    {
        private readonly Option<T4> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value) : base(value)
        {
        }

        public Union(T4 value)
        {
            this.internalValue = value;
        }

        Option<T4> IUnion<T4>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3, T4, T5> : Union<T1, T2, T3, T4>, IUnion, IUnion<T5>
    {
        private readonly Option<T5> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value) : base(value)
        {
        }

        public Union(T4 value) : base(value)
        {
        }

        public Union(T5 value)
        {
            this.internalValue = value;
        }

        Option<T5> IUnion<T5>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3, T4, T5, T6> : Union<T1, T2, T3, T4, T5>, IUnion, IUnion<T6>
    {
        private readonly Option<T6> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value) : base(value)
        {
        }

        public Union(T4 value) : base(value)
        {
        }

        public Union(T5 value) : base(value)
        {
        }

        public Union(T6 value)
        {
            this.internalValue = value;
        }

        Option<T6> IUnion<T6>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3, T4, T5, T6, T7> : Union<T1, T2, T3, T4, T5, T6>, IUnion, IUnion<T7>
    {
        private readonly Option<T7> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value) : base(value)
        {
        }

        public Union(T4 value) : base(value)
        {
        }

        public Union(T5 value) : base(value)
        {
        }

        public Union(T6 value) : base(value)
        {
        }

        public Union(T7 value)
        {
            this.internalValue = value;
        }

        Option<T7> IUnion<T7>.opt
        {
            get { return internalValue; }
        }
    }

    public class Union<T1, T2, T3, T4, T5, T6, T7, T8> : Union<T1, T2, T3, T4, T5, T6, T7>, IUnion, IUnion<T8>
    {
        private readonly Option<T8> internalValue;

        protected Union()
        { }

        public Union(T1 value) : base(value)
        {
        }

        public Union(T2 value) : base(value)
        {
        }

        public Union(T3 value) : base(value)
        {
        }

        public Union(T4 value) : base(value)
        {
        }

        public Union(T5 value) : base(value)
        {
        }

        public Union(T6 value) : base(value)
        {
        }

        public Union(T7 value) : base(value)
        {
        }

        public Union(T8 value)
        {
            this.internalValue = value;
        }

        Option<T8> IUnion<T8>.opt
        {
            get { return internalValue; }
        }
    }
}