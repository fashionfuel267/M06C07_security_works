using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace M06C07_security.Models
{
    public class dbHealthContext:IdentityDbContext<ApplicationUser>
    {
        public dbHealthContext(DbContextOptions<dbHealthContext>op):base(op) { }
       
        public DbSet<LoginModel> LoginModel { get; set; }

    }
    public class ApplicationUser : IdentityUser
    {

    }
}
