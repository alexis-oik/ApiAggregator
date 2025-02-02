using ApiAggregator.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ApiAggregator.Errors
{
    public sealed record Error(string Code, object? Description = null, ProblemDetails? ProblemDetails = null, IEnumerable? ValidationErrors = null)
    {
        public static readonly Error None = new(string.Empty);

        public static implicit operator Result(Error error) => Result.Failure(error);
    }
}
