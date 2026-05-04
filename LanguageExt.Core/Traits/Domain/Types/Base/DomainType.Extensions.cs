using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Provides deconstruction helpers for domain types backed by tuple-like representations.
/// </summary>
public static partial class DomainTypeExtensions
{
    extension<SELF, A>(SELF self)
        where SELF : DomainType<SELF, A>
    {
        /// <summary>
        /// Deconstructs the domain value into it underlying component.
        /// </summary>
        public void Deconstruct(out A a) =>
            a = self.To();
    }

    extension<SELF, A, B>(SELF self)
        where SELF : DomainType<SELF, (A, B)>
    {
        /// <summary>
        /// Deconstructs the domain value into its two underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b) =>
            (a, b) = self.To();
    }

    extension<SELF, A, B, C>(SELF self)
        where SELF : DomainType<SELF, (A, B, C)>
    {
        /// <summary>
        /// Deconstructs the domain value into its three underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b, out C c) =>
            (a, b, c) = self.To();
    }

    extension<SELF, A, B, C, D>(SELF self)
        where SELF : DomainType<SELF, (A, B, C, D)>
    {
        /// <summary>
        /// Deconstructs the domain value into its four underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b, out C c, out D d) =>
            (a, b, c, d) = self.To();
    }

    extension<SELF, A, B, C, D, E>(SELF self)
        where SELF : DomainType<SELF, (A, B, C, D, E)>
    {
        /// <summary>
        /// Deconstructs the domain value into its five underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b, out C c, out D d, out E e) =>
            (a, b, c, d, e) = self.To();
    }

    extension<SELF, A, B, C, D, E, F>(SELF self)
        where SELF : DomainType<SELF, (A, B, C, D, E, F)>
    {
        /// <summary>
        /// Deconstructs the domain value into its six underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b, out C c, out D d, out E e, out F f) =>
            (a, b, c, d, e, f) = self.To();
    }

    extension<SELF, A, B, C, D, E, F, G>(SELF self)
        where SELF : DomainType<SELF, (A, B, C, D, E, F, G)>
    {
        /// <summary>
        /// Deconstructs the domain value into its seven underlying components.
        /// </summary>
        public void Deconstruct(out A a, out B b, out C c, out D d, out E e, out F f, out G g) =>
            (a, b, c, d, e, f, g) = self.To();
    }
}
