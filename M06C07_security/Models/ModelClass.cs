using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace M06C07_security.Models
{
    public class TokenApiModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class LoginModel  
    
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        //public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
        //public string? CreatedBy { get; set; }
        //public DateTime UpdatedDate { get; set; }
        //public string? UpdatedBy
        //{
        //    get; set;
        //}
        
        //public bool IsActive { get; set; } = true;
    }
    
}
