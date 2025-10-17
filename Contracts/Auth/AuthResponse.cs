namespace bank_api.Contracts.Auth
{
    public sealed class AuthResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string TokenType { get; init; } = "Bearer";
        public DateTime ExpiresAtUtc { get; init; }
    }
}


