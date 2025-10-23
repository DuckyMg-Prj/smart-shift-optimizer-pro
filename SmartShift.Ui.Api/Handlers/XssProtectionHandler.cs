using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SmartShift.Ui.Api.Handlers
{
    /// <summary>
    /// Very small request inspector blocking obvious XSS patterns and sets security headers.
    /// Not a substitute for proper input validation & output encoding.
    /// </summary>
    public class XssProtectionHandler : DelegatingHandler
    {
        // Simple regex patterns to detect typical attack payloads. You can extend these.
        private static readonly Regex[] _suspiciousPatterns = new[] {
            new Regex(@"<\s*script\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"on\w+\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled), // onload= onerror= etc
            new Regex(@"javascript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"<\s*iframe\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"%3Cscript%3E", RegexOptions.IgnoreCase | RegexOptions.Compiled) // encoded
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Inspect headers and query for suspicious content
            var query = request.RequestUri.Query ?? "";
            var path = request.RequestUri.AbsolutePath ?? "";

            if (ContainsSuspiciousPatterns(query) || ContainsSuspiciousPatterns(path))
            {
                return CreateBadRequest(request, "malicious_request_detected");
            }

            // 2. Inspect body for suspicious content for likely JSON/text requests
            if (request.Content != null && IsTextMediaType(request.Content.Headers.ContentType?.MediaType))
            {
                var contentString = await request.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(contentString) && ContainsSuspiciousPatterns(contentString))
                {
                    return CreateBadRequest(request, "malicious_payload_detected");
                }

                // optionally, reassign sanitized content here (not done — prefer to reject)
            }

            // 3. Call inner handlers / controllers
            var response = await base.SendAsync(request, cancellationToken);

            // 4. Add security headers on response (CSP, X-Frame-Options, etc.)
            AddSecurityHeaders(response);

            return response;
        }

        private bool ContainsSuspiciousPatterns(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return _suspiciousPatterns.Any(r => r.IsMatch(input));
        }

        private bool IsTextMediaType(string mediaType)
        {
            if (string.IsNullOrEmpty(mediaType)) return false;
            return mediaType.StartsWith("text", StringComparison.OrdinalIgnoreCase) ||
                   mediaType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   mediaType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   mediaType.IndexOf("form", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private HttpResponseMessage CreateBadRequest(HttpRequestMessage request, string error)
        {
            var resp = request.CreateResponse(HttpStatusCode.BadRequest, new
            {
                error = error,
                error_description = "Request was rejected due to suspicious content."
            });
            return resp;
        }

        private void AddSecurityHeaders(HttpResponseMessage response)
        {
            if (response == null || response.Headers == null) return;

            // Basic security headers - tune CSP for your frontend needs
            if (!response.Headers.Contains("X-Content-Type-Options"))
                response.Headers.Add("X-Content-Type-Options", "nosniff");

            if (!response.Headers.Contains("X-Frame-Options"))
                response.Headers.Add("X-Frame-Options", "DENY");

            if (!response.Headers.Contains("Referrer-Policy"))
                response.Headers.Add("Referrer-Policy", "no-referrer");

            // Content-Security-Policy (adjust if you load scripts from CDN)
            if (!response.Headers.Contains("Content-Security-Policy"))
            {
                var csp = "default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none';";
                response.Headers.Add("Content-Security-Policy", csp);
            }

            if (!response.Headers.Contains("X-XSS-Protection"))
                response.Headers.Add("X-XSS-Protection", "1; mode=block");
        }
    }
}
