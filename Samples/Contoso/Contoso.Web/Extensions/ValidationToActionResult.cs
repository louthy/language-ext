using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Extensions
{
    public static class ValidationToActionResult
    {
        public static IActionResult ToActionResult<T>(this Validation<Error, T> validation) =>
            validation.Match<IActionResult>(
                Succ: t => new OkObjectResult(t),
                Fail: e => new BadRequestObjectResult(e));

        public static Task<IActionResult> ToActionResult<T>(this Task<Validation<Error, T>> validation)
            => validation.Map(ToActionResult);
    }
}
