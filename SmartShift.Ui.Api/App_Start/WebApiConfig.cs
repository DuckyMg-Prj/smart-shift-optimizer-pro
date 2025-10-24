using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SmartShift.Ui.Api.Handlers;
namespace SmartShift.Ui.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new IpRateLimitHandler());
            config.MessageHandlers.Add(new XssProtectionHandler());
            config.MessageHandlers.Add(new LanguageHandler());

            // existing routes/config
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
