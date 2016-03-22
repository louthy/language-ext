using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ParserResult
    {
        public static ParserResult<T> EmptyOK<T>(T value, PString input, ParserError error = null) =>
            new ParserResult<T>(ResultTag.Empty, Reply.OK(value, input, error));

        public static ParserResult<T> EmptyError<T>(Pos pos, string message) =>
            new ParserResult<T>(ResultTag.Empty, Reply.Error<T>(PString.Zero, pos, message, List.empty<string>()));

        public static ParserResult<T> EmptyError<T>(ParserError error) =>
            new ParserResult<T>(ResultTag.Empty, Reply.Error<T>(PString.Zero, error));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input, ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input, error));

        public static ParserResult<T> ConsumedError<T>(PString input, Pos pos, string message, Lst<string> expected) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.Error<T>(input, pos, message, expected));

        public static ParserResult<T> ConsumedError<T>(PString input, ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.Error<T>(input, error));

    }

    public enum ResultTag
    {
        Consumed,
        Empty
    }

    public enum ReplyTag
    {
        OK,
        Error
    }

    public class ParserResult<T>
    {
        public readonly ResultTag Tag;
        public readonly Reply<T> Reply;

        internal ParserResult(ResultTag tag, Reply<T> reply)
        {
            Tag = tag;
            Reply = reply;
        }

        public override string ToString() =>
            IsFaulted
                ? $"parse error at (line {Reply.Error.Pos.Line}, column {Reply.Error.Pos.Column}):\n" +
                  (String.IsNullOrEmpty(Reply.Error.Message)
                     ? $"expecting: {String.Join(", ", Reply.Error.Expected)}\n"
                     : $"{Reply.Error.Message}\n" +
                       $"expecting: {String.Join(", ", Reply.Error.Expected)}")
                : "success";

        public bool IsFaulted =>
            Reply.Tag == ReplyTag.Error;

        public R Match<R>(
            Func<ParserResult<T>, R> Consumed,
            Func<T, PString, ParserError, R> EmptyOK,
            Func<ParserError, R> EmptyError
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.OK)
            {
                return EmptyOK(Reply.Result, Reply.Remaining, Reply.Error);
            }
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            return Consumed(this);
        }

        public R Match<R>(
            Func<T, PString, ParserError, R> ConsumedOK,
            Func<PString, ParserError, R> ConsumedError,
            Func<T, PString, ParserError, R> EmptyOK,
            Func<ParserError, R> EmptyError
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.OK)
            {
                return EmptyOK(Reply.Result, Reply.Remaining, Reply.Error);
            }
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            if (Tag == ResultTag.Consumed && Reply.Tag == ReplyTag.OK)
            {
                return ConsumedOK(Reply.Result, Reply.Remaining, Reply.Error);
            }
            return ConsumedError(Reply.Remaining, Reply.Error);
        }
    }

    public static class Reply
    {
        public static Reply<T> OK<T>(T result, PString remaining, ParserError error = null) =>
            new Reply<T>(result, remaining, error);

        public static Reply<T> Error<T>(PString remaining, Pos pos, string message, Lst<string> expected) =>
            new Reply<T>(remaining, new ParserError(pos, message, expected));

        public static Reply<T> Error<T>(PString remaining, ParserError error) =>
            new Reply<T>(remaining, error);
    }

    public class Reply<T>
    {
        public readonly ReplyTag Tag;
        public readonly T Result;
        public readonly PString Remaining;
        public readonly ParserError Error;

        internal Reply(PString remaining, ParserError error)
        {
            Tag = ReplyTag.Error;
            Error = error;
            Remaining = remaining;
        }

        internal Reply(T result, PString remaining, ParserError error = null)
        {
            Tag = ReplyTag.OK;
            Remaining = remaining;
            Result = result;
            Error = error;
        }
    }
}
