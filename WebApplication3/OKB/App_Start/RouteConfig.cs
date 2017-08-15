using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OKB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //   routes.MapRoute(
            //    "Default",
            //    //url: "{controller}/{action}/{id}",
            //    "{controller}/{action}/{id}",
            //           //defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            //           new
            //           {
            //               controller = "User",
            //               action = "Login",
            //               id = UrlParameter.Optional
            //           }, new { id = @"\d+" }
            //);

            routes.MapRoute(
                name: "Default",
                //url: "{controller}/{action}/{id}",
                url: "{controller}/{action}/{id}",
                 //defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
                 defaults: new
                 {
                     controller = "User",
                     action = "Login",
                     id = UrlParameter.Optional
                 }//new { id = @"\d+" }
            );
        }
    }
}
