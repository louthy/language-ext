using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.DataTypes.Recursive
{
    /// <summary>
    /// While the .NET CLR supports tail call optimisation, the C# designers made the decision to limit the depth of the call stack.
    /// This class allows recursive functions to have a near unlimited stack depth.    
    /// This class is implemented using trampoling and implicit type conversion.   
    /// </summary>    
    /// <remarks>
    /// Intended usage is for situations where an algorithm might more conveniently be expressed as 
    /// a recursive function but fears this may result in a stack overflow error.
    /// 
    /// Sample 
    /// 
    /// Note: This is a lazy type. The function is only invoked when an implicit cast occurs    
    /// </remarks>
    /// <typeparam name="A">Base case return type</typeparam>
    public class Recursive<A>
    {
        private enum State
        {
            Recurse,
            Return
        }

        private readonly State recursiveState;
        private readonly Func<Recursive<A>> recursiveFunction;
        private readonly A val;

        internal Recursive(Some<A> val)
        {
            this.val = val;
            recursiveFunction = default(Func<Recursive<A>>);
            recursiveState = State.Return;
        }

        internal Recursive(Func<Recursive<A>> func)
        {
            recursiveState = State.Recurse;
            recursiveFunction = func;
            val = default(A);
        }

        public static implicit operator A(Recursive<A> val)
        {
            // Here is where the trampolining (and execution takes place).
            // The `current` function is invoked which yields either a result of type 'A or the next function to invoke.
            
            Some<Recursive<A>> current = val;
            while (current.Value.recursiveState != State.Return)
            {
                current = current.Value.recursiveFunction();
            }
            return current.Value.val;
        }

        [Pure]
        public static implicit operator Recursive<A>(A val) => new Recursive<A>(val);        

        [Pure]
        public Type GetUnderlyingType() => typeof(A);
        
    }
}