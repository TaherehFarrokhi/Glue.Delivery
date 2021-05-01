using System;
using Glue.Delivery.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Glue.Delivery.WebApi.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult<T> ToActionResult<T>(this OperationResult<T> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.Succeed)
                return new OkObjectResult(result.Result);

            return new StatusCodeResult(500);
        }
    }
}