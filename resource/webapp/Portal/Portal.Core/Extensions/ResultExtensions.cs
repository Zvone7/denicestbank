using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace Portal.Core.Extensions;

public static class ResultExtensions
{
    public static Result<TR> Match<T, TR>(
        this Result<T> result,
        Func<T, Result<TR>> success)
    {
        return result.Match(
            success,
            exception => new Result<TR>(exception));
    }

    public static async Task<Result<TR>> MatchAsync<T, TR>(
        this Result<T> result,
        Func<T, Task<Result<TR>>> success)
    {
        return await result.Match(
            async succ => await success(succ),
            e => Task.FromResult(new Result<TR>(e)));
    }
    
    public static async Task<Result<TR>> MatchAsync<T, TR>(
        this Result<T> result,
        Func<T, Task<Result<TR>>> success,
        Func<Exception, Task<Result<TR>>> failure)
    {
        return await result.Match(
            async succ => await success(succ),
            async fail => await failure(fail));
    }

    public static Result<TR> Bind<T, TR>(this Result<T> result, Func<T, Result<TR>> success)
    {
        return result.Match(
            v => success(v),
            e => new Result<TR>(e));
    }

    public static async Task<Result<TR>> BindAsync<T, TR>(this Result<T> result, Func<T, Task<Result<TR>>> success)
    {
        var rr = await result.MapAsync(success);
        return rr.Bind(r => r);
    }
}