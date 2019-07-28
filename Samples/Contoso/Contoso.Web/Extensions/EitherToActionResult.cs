using System.Threading.Tasks;
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

        public static async Task<IActionResult> ToActionResult<L, R>(this Task<Either<L, R>> either) =>
            (await either).ToActionResult();
    }
}
