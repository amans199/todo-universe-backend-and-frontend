using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using todo_universe.ActionFilters;

namespace todo_universe.Controllers;

[ApiController]
[Authorize] // Uses authentication scheme to determine user
[ServiceFilter(typeof(UserIdActionFilter))] // Pulls user ID from claims
public abstract class ApiController : ControllerBase
{
    public string UserId { get; set; }
}
