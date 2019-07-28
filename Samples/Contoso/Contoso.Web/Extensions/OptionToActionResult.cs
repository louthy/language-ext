using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Web.Extensions
{
    public static class OptionToActionResult
    {
        public static async Task<IActionResult> ToActionResult<T>(this Task<Option<T>> option) =>
            (await option).ToActionResult();

        public static IActionResult ToActionResult<T>(this Option<T> option) =>
            option.Match<IActionResult>(
                Some: t => new OkObjectResult(t),
                None: () => new NotFoundResult());
    }
}
