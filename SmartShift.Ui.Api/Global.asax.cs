using System;
using System.Globalization;
using System.Threading;
using System.Web;
using SmartShift.Core.Service;
using System.Web.Http;
using Unity.WebApi;
namespace SmartShift.Ui.Api
{


    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var langHeader = HttpContext.Current.Request.Headers["Accept-Language"];
            if (!string.IsNullOrEmpty(langHeader))
            {
                try
                {

                    var lang = langHeader.Split(',')[0].Trim();
                    var culture = CultureInfo.GetCultureInfo(lang);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                catch
                {
                    // ignore invalid culture values and keep default
                }
            }
        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // get container from your service layer
            Unity.IUnityContainer container = UnityConfig.GetConfiguredContainer();

            // set resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }

}



