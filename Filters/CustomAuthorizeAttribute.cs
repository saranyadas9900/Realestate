using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication4.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Check if the user is authenticated
            if (filterContext.HttpContext.Session["Username"] == null || filterContext.HttpContext.Session["UserId"] == null)
            {
                // If not authenticated, redirect to signin page
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Auth", action = "SignIn" }));
            }
            else
            {
                // If authenticated, proceed with the request
                base.OnAuthorization(filterContext);
            }
        }
    }
}