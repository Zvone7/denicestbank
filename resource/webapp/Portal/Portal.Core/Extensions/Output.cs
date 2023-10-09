using System.Collections.Immutable;

namespace Portal.Core.Extensions;

public record Output<T1, T2>
{
    public ImmutableList<T1> Results { get; init; } = ImmutableList<T1>.Empty;
    public ImmutableList<T2> Errors { get; init; } = ImmutableList<T2>.Empty;
}