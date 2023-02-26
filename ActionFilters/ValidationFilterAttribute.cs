using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Linq;
using todo_universe.Models;
using System.Security.Authentication;
using System.Security.Claims;

namespace todo_universe.ActionFilters;

public class ValidationFilterAttribute :IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {

        //get username from claims 
        var userName = context.HttpContext.User.Identity.Name;

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null)
        {
            throw new AuthenticationException("User not found");
        }

        if (userName == null)
        {
            context.Result = new BadRequestObjectResult("User Is unauthorized");
            return;
        }

        //var param = context.ActionArguments.SingleOrDefault(x => x.Value is ITodo);
            
        //if (param.Value == null)
        //{
        //    context.Result = new BadRequestObjectResult("Object is null");
        //    return;
        //}

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}
