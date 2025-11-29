using System;
using System.Threading;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Makes `K〈F, A〉` lazy
/// </summary>
/// <remarks>
/// This is more of a utility type right now, so it hasn't been fleshed out like other Applicatives
/// </remarks>
public class Memo<F, A>
{
    /// <summary>
    /// Acquired value or null if not yet acquired
    /// </summary>
    K<F, A>? value;
    
    /// <summary>
    /// Function to acquire the value
    /// </summary>
    Func<K<F, A>>? acquire;
    
    /// <summary>
    /// 0 = unloaded, 1 = loading, 2 = loaded
    /// </summary>
    int state;
    
    /// <summary>
    /// Construct a memo
    /// </summary>
    /// <param name="acq">Value acquisition function</param>
    internal Memo(Func<K<F, A>> f)
    {
        value = null;
        acquire = f;
        state = 0;
    }
    
    /// <summary>
    /// Construct a memo
    /// </summary>
    /// <param name="fa">Already acquired value</param>
    internal Memo(Func<K<F, A>>? f, K<F, A>? fa)
    {
        value = fa;
        acquire = f;
        state = 2;
    }
    
    /// <summary>
    /// Construct a memo
    /// </summary>
    /// <param name="fa">Already acquired value</param>
    internal Memo(K<F, A> fa)
    {
        value = fa;
        acquire = null;
        state = 2;
    }

    public Memo<K<F, A>> Lower() =>
        new (() => Value);
    
    /// <summary>
    /// Acquired value
    /// </summary>
    /// <remarks>
    /// If the value is not yet acquired, it will be acquired within `Value` and cached.
    /// </remarks>
    public K<F, A> Value => 
        state == 2
            ? value!
            : GetValue();

    /// <summary>
    /// Create a clone of the memo (without any cached results).  This allows for reacquiring the value.
    /// </summary>
    /// <returns>A newly constructed memo structure</returns>
    public Memo<F, A> Clone() =>
        acquire is null
            ? new(value!)
            : new(acquire);
    
    /// <summary>
    /// Reset the memo.  This clears the cached value. 
    /// </summary>
    /// <returns>Unit</returns>
    public Unit Reset()
    {
        if(acquire is null) return default;
        SpinWait sw = default;
        while (true)
        {
            switch (Interlocked.CompareExchange(ref state, 1, 2))
            {
                case 0:
                    // unloaded
                    return default;
                
                case 1:
                    // currently unloading, so wait, then loop around and try again
                    sw.SpinOnce();
                    break;
                
                case 2:
                    value = null;
                    state = 0;
                    return default;

                default:
                    throw new InvalidOperationException("Invalid state");
            }
        }
    }
    
    /// <summary>
    /// Acquisition function.  Called only if the value is not yet acquired.
    /// </summary>
    K<F, A> GetValue()
    {
        SpinWait sw = default;
        while (true)
        {
            switch (Interlocked.CompareExchange(ref state, 1, 0))
            {
                case 0:
                    // value is not set yet, so try to set it
                    try
                    {
                        value = acquire!();
                        state = 2;
                        return value;
                    }
                    catch
                    {
                        // if we fail, reset the state to 'not set yet'
                        state = 0;
                        throw;
                    }
                    
                case 1:
                    // currently loading, so wait, then loop around and try again
                    sw.SpinOnce();
                    break;
                    
                case 2:
                    // value already set
                    return value!;

                default:
                    throw new InvalidOperationException("Invalid state");
            }
        }
    }
}
