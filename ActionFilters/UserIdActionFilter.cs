using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using todo_universe.Controllers;

namespace todo_universe.ActionFilters;
public class UserIdActionFilter  : Attribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }

    // Pull the user ID on each request
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var c = context.Controller as ApiController;
        c.UserId = c.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
