using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Web.Testing.Extensions
{
    public static class ActionResultExtensions
    {
        public static TResult GetResultValue<TResult>(this ActionResult<TResult> actionResult) where TResult : class
        {
            var okResult = actionResult.Result as OkObjectResult;

            okResult.ShouldNotBeNull();

            var result = okResult.Value as TResult;

            result.ShouldNotBeNull();

            return result;
        }
    }
}
