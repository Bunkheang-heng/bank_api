namespace bank_api.Contracts.Auth
{
    public sealed class RegisterRequest
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}


