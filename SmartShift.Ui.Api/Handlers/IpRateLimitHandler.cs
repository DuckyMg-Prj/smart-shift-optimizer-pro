using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace SmartShift.Ui.Api.Handlers
{
    public class IpRateLimitHandler : DelegatingHandler
    {
        // Simple in-memory cache for counters. Replace with Redis for production.
        private static readonly MemoryCache _cache = MemoryCache.Default;

        // Tune these values
        private readonly int _globalLimit = 200;               // requests per window per IP
        private readonly int _authEndpointLimit = 10;          // stricter for auth endpoints
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var ip = GetClientIp(request) ?? "unknown";
                var key = $"rl:{ip}";
                var limit = _globalLimit;

                // If request is to token or auth endpoints, use stricter limit
                var path = request.RequestUri.AbsolutePath?.ToLowerInvariant() ?? "";
                if (path.Contains("/api/auth/token") || path.Contains("/api/auth/login") || path.Contains("/api/auth/register"))
                    limit = _authEndpointLimit;

                var entry = (RateLimitEntry)_cache.Get(key);
                if (entry == null)
                {
                    entry = new RateLimitEntry { Count = 1, Expires = DateTime.UtcNow.Add(_window) };
                    _cache.Set(key, entry, entry.Expires);
                }
                else
                {
                    if (entry.Count >= limit)
                    {
                        var retryAfterSeconds = (int)(entry.Expires - DateTime.UtcNow).TotalSeconds;
                        var resp = request.CreateResponse((HttpStatusCode)429, new
                        {
                            error = "too_many_requests",
                            error_description = "Rate limit exceeded. Try again later."
                        });
                        resp.Headers.Add("Retry-After", retryAfterSeconds.ToString());
                        return Task.FromResult(resp);
                    }
                    entry.Count++;
                    // refresh expiration to original window start (do not extend)
                    _cache.Set(key, entry, entry.Expires);
                }
            }
            catch
            {
                // don't block request on rate limiter error — fail open
            }

            return base.SendAsync(request, cancellationToken);
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            // Try common headers (behind proxy)
            if (request.Properties.TryGetValue("MS_HttpContext", out var ctxObj) && ctxObj is System.Web.HttpContextWrapper ctx)
                return ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? ctx.Request.UserHostAddress;

            if (request.Headers.TryGetValues("X-Forwarded-For", out var vals))
                return vals?.FirstOrDefault();

            if (request.Properties.TryGetValue("RemoteEndpointMessageProperty", out var remoteEndpoint))
            {
                dynamic prop = remoteEndpoint;
                return prop?.Address;
            }

            return null;
        }

        private class RateLimitEntry
        {
            public int Count;
            public DateTime Expires;
        }
    }
}
