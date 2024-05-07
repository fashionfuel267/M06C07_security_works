namespace M06C07_security.Utility
{
    public class AuthenticatedResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }
    }
}
