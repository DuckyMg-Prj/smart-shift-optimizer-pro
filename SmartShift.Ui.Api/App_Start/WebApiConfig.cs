
using SmartShift.Ui.Api.Handlers;
using System.Web.Http;
using System.Web.Http.Cors;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.MessageHandlers.Add(new IpRateLimitHandler());
        config.MessageHandlers.Add(new XssProtectionHandler());
        config.MessageHandlers.Add(new LanguageHandler());

        var cors = new EnableCorsAttribute("*", "*", "*");
        config.EnableCors(cors);

        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );
    }
}


