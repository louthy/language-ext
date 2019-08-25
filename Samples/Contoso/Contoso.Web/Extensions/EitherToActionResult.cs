using System.Threading.Tasks;
using Contoso.Core;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Extensions
{
    public static class EitherToActionResult
    {
        public static IActionResult ToActionResult<L, R>(this Either<L, R> either) =>
            either.Match<IActionResult>(
                Left: l => new BadRequestObjectResult(l),
                Right: r => new OkObjectResult(r));

        public static Task<IActionResult> ToActionResult<L, R>(this Task<Either<L, R>> either) =>
            either.Map(ToActionResult);

        public static Task<IActionResult> ToActionResult(this Task<Either<Error, Task>> either) =>
            either.Bind(e =>
                e.MapLeft(l => new BadRequestObjectResult(l))
                .MapAsync(_ => new OkResult().AsTask())
                .Map(ToActionResult));
    }
}
