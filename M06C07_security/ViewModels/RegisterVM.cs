namespace M06C07_security.ViewModels
{
    
    public class RegisterVM
    {
        public string? UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        //public string RolesName { get; set; }

        public string Password { get; set; }
    }
    public class LoginVM
    {
        public string? UserName { get; set; }
        //public string RolesName { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
