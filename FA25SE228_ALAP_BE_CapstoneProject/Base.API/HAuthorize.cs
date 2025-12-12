using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Base.API
{
    public class HAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public HAuthorize(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated || !_roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult(); // Trả về 403 nếu không có quyền
            }
        }
    }
}
