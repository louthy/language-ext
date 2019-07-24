using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public delegate (A Value, bool IsFaulted) Reader<Env, A>(Env env);
}