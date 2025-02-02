using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ApiAggregator.Errors
{
    public class GlobalErrors
    {
        public static Error GlobalExceptionError(ProblemDetails problemDetails) => new("GlobalErrors.GlobalExceptionError", "", problemDetails);

    }
}
