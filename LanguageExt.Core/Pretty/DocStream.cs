using System;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    public abstract record DocStream<A>
    {
        public DocStream<B> Select<B>(Func<A, B> f) => ReAnnotate(f);
        public DocStream<B> Map<B>(Func<A, B> f) => ReAnnotate(f);
        public abstract DocStream<B> ReAnnotate<B>(Func<A, B> f);

        public string Show()
        {
            var doc = this;
            var sb  = new StringBuilder();
            while (true)
            {
                switch (doc)
                {
                    case SFail<A>:
                        sb.Append("[FAIL]");
                        return sb.ToString();//throw new Exception("Doc Fail");
                    
                    case SEmpty<A>:
                        return sb.ToString();
                    
                    case SChar<A> (var c, var next):
                        sb.Append(c);
                        doc = next;
                        break;
                    
                    case SText<A> (var t, var next):
                        sb.Append(t);
                        doc = next;
                        break;
                    
                    case SLine<A>(var i, var next):
                        sb.AppendLine();
                        sb.Append(FastSpace.Show(i));
                        doc = next;
                        break;
                    
                    case SAnnPush<A>(var _, var next):
                        doc = next;
                        break;
                    
                    case SAnnPop<A>(var next):
                        doc = next;
                        break;
                }
            }
        }
    }

    public record SFail<A> : DocStream<A>
    {
        public static readonly DocStream<A> Default = new SFail<A>();

        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            SFail<B>.Default;
    }

    public record SEmpty<A> : DocStream<A>
    {
        public static readonly DocStream<A> Default = new SEmpty<A>();

        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            SEmpty<B>.Default;
    }

    public record SChar<A>(char Value, DocStream<A> Next) : DocStream<A>
    {
        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            new SChar<B>(Value, Next.ReAnnotate(f));
    }

    public record SText<A>(string Value, DocStream<A> Next) : DocStream<A>
    {
        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            new SText<B>(Value, Next.ReAnnotate(f));
    }
    
    public record SLine<A>(int Indent, DocStream<A> Next) : DocStream<A>
    {
        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            new SLine<B>(Indent, Next.ReAnnotate(f));
    }
        
    public record SAnnPush<A>(A Ann, DocStream<A> Next) : DocStream<A>
    {
        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            new SAnnPush<B>(f(Ann), Next.ReAnnotate(f));
    }

    public record SAnnPop<A>(DocStream<A> Next) : DocStream<A>
    {
        public override DocStream<B> ReAnnotate<B>(Func<A, B> f) =>
            new SAnnPop<B>(Next.ReAnnotate(f));
    }
}
