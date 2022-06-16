using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace LanguageExt
{
    public static partial class Prelude
    {
        // There is no documentation that specifies whether the underlying RNG is thread-safe.
        // It is assumed that the implementation is `RNGCryptoServiceProvider`, which under the
        // hood calls `CryptGenRandom` from `advapi32` (i.e. a built-in Windows RNG).  There is
        // no mention of thread-safety issues in any documentation, so we assume this must be 
        // thread-safe.
        //
        // Documentation of `CrypGenRandom`
        //
        //    https://docs.microsoft.com/en-us/windows/win32/api/wincrypt/nf-wincrypt-cryptgenrandom
        //
        // There is some discussion here:
        //
        //    https://stackoverflow.com/questions/46147805/is-cryptgenrandom-thread-safe
        //
        static readonly RandomNumberGenerator rnd = RandomNumberGenerator.Create();
        static readonly int wordTop = BitConverter.IsLittleEndian ? 3 : 0;

        /// <summary>
        /// Thread-safe cryptographically strong random number generator
        /// </summary>
        /// <param name="max">Maximum value to return + 1</param>
        /// <returns>A non-negative random number, less than the value specified.</returns>
        public static int random(int max)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(4);
            rnd.GetBytes(bytes);
            bytes[wordTop] &= 0x7f;
            var value = BitConverter.ToInt32(bytes, 0) % max;
            ArrayPool<byte>.Shared.Return(bytes);
            return value;
        }

        /// <summary>
        /// Thread-safe cryptographically strong random base-64 string generator
        /// </summary>
        /// <param name="bytesCount">number of bytes generated that are then 
        /// returned Base64 encoded</param>
        /// <returns>Base64 encoded random string</returns>
        public static string randomBase64(int bytesCount)
        {
            if (bytesCount < 1) throw new ArgumentException($"The minimum value for {nameof(bytesCount)} is 1");
            var bytes = ArrayPool<byte>.Shared.Rent(bytesCount);
            rnd.GetBytes(bytes);
            var r = Convert.ToBase64String(bytes);
            ArrayPool<byte>.Shared.Return(bytes);
            return r;
        }
    }
}
