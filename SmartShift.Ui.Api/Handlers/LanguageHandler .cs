using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartShift.Ui.Api.Handlers
{
    /// <summary>
    /// Reads Accept-Language header and sets CurrentUICulture.
    /// </summary>
    public class LanguageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var langHeader = request.Headers.AcceptLanguage.FirstOrDefault()?.Value;

            if (!string.IsNullOrEmpty(langHeader))
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(langHeader);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                catch (CultureNotFoundException)
                {
                    // fallback to English
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
