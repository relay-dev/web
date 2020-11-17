using Microsoft.AspNetCore.Mvc;

namespace Web.Extensions
{
    public static class ActionResultExtensions
    {
        public static TResult GetResultValue<TResult>(this IActionResult actionResult) where TResult : class
        {
            var okResult = actionResult as OkObjectResult;

            return okResult?.Value as TResult;
        }

        public static TResult GetResultValue<TResult>(this ActionResult<TResult> actionResult) where TResult : class
        {
            var okResult = actionResult.Result as OkObjectResult;

            return okResult?.Value as TResult;
        }
    }
}
