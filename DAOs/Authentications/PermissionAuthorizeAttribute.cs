using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.Authentications
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private int[] _roles;

        public PermissionAuthorizeAttribute(params int[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var roleClaim = context.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type.ToLower() == ClaimTypes.Role.ToLower());

                if (roleClaim == null || !int.TryParse(roleClaim.Value, out var roleValue) || !_roles.Contains(roleValue))
                {
                    context.Result = new StatusCodeResult(403);
                }
            }
            else
            {
                context.Result = new StatusCodeResult(401);
            }
        }
    }
}
