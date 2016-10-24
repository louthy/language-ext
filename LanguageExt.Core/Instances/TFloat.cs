using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    public struct TFloat : Ord<float>, Floating<float>
    {
        public static readonly TFloat Inst = default(TFloat);

        public bool Equals(float x, float y) => x == y;

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        public int Compare(float x, float y) =>
            x.CompareTo(y);

        public float Plus(float x, float y) => x + y;
        public float Subtract(float x, float y) => x - y;
        public float Product(float x, float y) => x * y;

        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        public float Divide(float x, float y) =>
            x / y;

        public float Abs(float x) => Math.Abs(x);
        public float Signum(float x) => Math.Sign(x);
        public float FromInteger(int x) => (float)x;
        public float FromDecimal(decimal x) => (float)x;
        public float FromFloat(float x) => (float)x;
        public float FromDouble(double x) => (float)x;
        public float Div(float x, float y) => x / y;
        public float FromRational(Ratio<int> x) => x.Numerator / x.Denominator;
        public float Pi() => (float)Math.PI;
        public float Exp(float x) => (float)Math.Exp(x);
        public float Sqrt(float x) => (float)Math.Sqrt(x);
        public float Log(float x) => (float)Math.Log(x);
        public float Pow(float x, float y) => (float)Math.Pow(x, y);
        public float LogBase(float b, float x) => (float)Math.Log(x, b);
        public float Sin(float x) => (float)Math.Sin(x);
        public float Cos(float x) => (float)Math.Cos(x);
        public float Tan(float x) => (float)Math.Tan(x);
        public float Asin(float x) => (float)Math.Asin(x);
        public float Acos(float x) => (float)Math.Acos(x);
        public float Atan(float x) => (float)Math.Atan(x);
        public float Sinh(float x) => (float)Math.Sinh(x);
        public float Cosh(float x) => (float)Math.Cosh(x);
        public float Tanh(float x) => (float)Math.Tanh(x);
        public float Asinh(float x) => (float)Math.Log(x + Math.Sqrt((x * x) + 1.0));
        public float Acosh(float x) => (float)Math.Log(x + Math.Sqrt((x * x) - 1.0));
        public float Atanh(float x) => 0.5f * (float)Math.Log((1.0 + x) / (1.0 - x));
        public float Append(float x, float y) => x + y;
    }
}
