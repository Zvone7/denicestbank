using FluentAssertions;
using LanguageExt.Common;

namespace Test.Portal.Core.Extensions;

public static class TestResultExtensions
{
    public static void ShouldHaveException<T>(this Result<T> result, Exception e)
    {
        result.Match(
            succ =>
            {
                succ.Should().BeNull();
                return false;
            },
            exception =>
            {
                exception.Should().Be(exception);
                return true;
            }).Should().BeTrue();
    }


    public static T GetValue<T>(this Result<T> result)
    {
        var res = result.Match(v => v, e => default(T));
        res.Should().NotBeNull($"Result<{typeof(T).Name}> should have a value");
        return res!;
    }
}