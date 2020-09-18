using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microservices.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

// na na middleware 
// phai khai bao trong start up 
namespace Microservices.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userID = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userID);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}