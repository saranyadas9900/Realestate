using System;
using System.Web;
using System.Web.Mvc;

public class NoCacheAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        filterContext.HttpContext.Response.Cache.SetNoStore();
        filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        filterContext.HttpContext.Response.Cache.SetNoStore();
        base.OnResultExecuting(filterContext);
    }
}
