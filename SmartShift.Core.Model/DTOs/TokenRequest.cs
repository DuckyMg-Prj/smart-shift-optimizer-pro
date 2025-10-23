namespace SmartShift.Core.Model.DTOs
{
    public class TokenRequest
    {
        /// <summary>
        /// Type of grant. Can be "password" or "refresh_token".
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// User email or username (for password grant)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User password (for password grant)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Refresh token string (for refresh_token grant)
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Optional client ID for tracking (mobile, web, etc.)
        /// </summary>
        public string ClientId { get; set; }
    }
}
