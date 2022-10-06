using Covid.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covid.Presentation.Helper
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.QueryString.Count > 1)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            else
            {
                if (SessionHelper.UserDetails == null)
                {
                    filterContext.Result = new RedirectResult("~/Login/LogIn");
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}