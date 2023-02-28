using System;
using Sprache;

namespace OctoAwesome.Client;

public static class SpracheExtensions
{
    public static Parser<TU> SelectWhere<T, TU>(this Parser<T> parser, Func<T, (bool match, TU value)> selectConvert)
    {
        if (parser == null)
            throw new ArgumentNullException(nameof(parser));
        if (selectConvert == null)
            throw new ArgumentNullException(nameof(selectConvert));
        return parser.Then((Func<T, Parser<TU>>) (t =>
                                                  {
                                                      var (match, value) = selectConvert(t);
                                                      return match
                                                          ? Parse.Return(value)
                                                          : i => Result.Failure<TU>(i, "Failed to convert", Array.Empty<string>());
                                                  }));
    }

    public static Parser<(bool isClosed, T value)> ContainedOptional<T, U, V>(
        this Parser<T> parser,
        Parser<U> open,
        Parser<V> close)
    {
        if (parser == null)
            throw new ArgumentNullException(nameof (parser));
        if (open == null)
            throw new ArgumentNullException(nameof (open));
        if (close == null)
            throw new ArgumentNullException(nameof (close));
        return open.SelectMany((Func<U, Parser<T>>) (o => parser), (o, item) => new { o, item })
            .SelectMany(_ => close.Optional(), (param1, c) => (c.IsDefined, param1.item));
    }
}