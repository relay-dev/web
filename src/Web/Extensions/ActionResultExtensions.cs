using Microsoft.AspNetCore.Mvc;
using System;

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

        public static TModel GetResultModel<TModel>(this IActionResult actionResult)
        {
            if (actionResult == null)
            {
                throw new InvalidOperationException("actionResult cannot be null");
            }

            var viewResult = actionResult as ViewResult;

            if (viewResult == null)
            {
                throw new InvalidOperationException("actionResult was not a ViewResult");
            }

            return (TModel)viewResult.Model;
        }
    }
}
