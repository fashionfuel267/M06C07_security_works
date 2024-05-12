using M06C07_security.Models;
using M06C07_security.Utility;
using M06C07_security.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EntityFrameworkCore.RawSQLExtensions.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Cors;
namespace M06C07_security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("res")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenmanager;
        dbHealthContext _context;
        public AuthenticateController(UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    ITokenService tokenmanager ,
                                    dbHealthContext db)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _tokenmanager = tokenmanager;
            _context=db;
          
        }
         
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Register")]
        public async Task<IActionResult> Register([Microsoft.AspNetCore.Mvc.FromBody] RegisterVM model)
        {
            try
            {
                var isExist = await _userManager.FindByEmailAsync(model.Email);
                if (isExist != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,"Email exist, try different email");
                }
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else if (result.Errors.Count() > 0)
                {
                    string msg = "";

                    foreach (var item in result.Errors)
                    {
                        msg += item.Code + "-" + item.Description + ",";

                    }
                    msg = msg.Substring(0, msg.Length - 1);
                    return StatusCode(StatusCodes.Status500InternalServerError, msg);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.InnerException.Message??ex.Message);
            }
        }
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Login")]

        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (loginVM == null)
            {
                return BadRequest("Invalid login request");
            }
            else
            {
                var existUser = await _userManager.FindByNameAsync(loginVM.UserName);
                if (existUser is null)
                {
                    return Unauthorized();
                }
                else
                {
                    var role = _userManager.GetRolesAsync(existUser).Result.FirstOrDefault();
                    var u = await _userManager.CheckPasswordAsync(existUser, loginVM.Password);
                   if(u)
                    {

                   
                    var claims = new List<Claim> {
                        new Claim (ClaimTypes.Name,loginVM.UserName??""),
                        new Claim (ClaimTypes.Role,role??""),
                         //new Claim (type:"Institute",existUser.InsId.ToString()),
                    };
                    string acctokens = _tokenmanager.GenerateAccessToken(claims);
                    var refreshToken = _tokenmanager.GenerateRefreshToken();
                    //var loggedUser = _unitofWork.LoginModelRepository
                    //                            .GetAll(L => L.UserName == loginVM.UserName 
                    //                                                        && L.DomainName.ToLower().Equals(host)
                    //                                                           , null).FirstOrDefault();
                    var loggedUser = _context.LoginModel.Where(u=>u.UserName.Equals(loginVM.UserName)).FirstOrDefault();
                                               
                    if (loggedUser != null)
                    {
                        loggedUser.RefreshToken = refreshToken;
                        loggedUser.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10);
                        _context.LoginModel.Update(loggedUser);
                    }
                    else
                    {
                        var inuser = new LoginModel
                        {
                            RefreshToken = refreshToken,
                            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10),
                            UserName = loginVM.UserName,
                            Password = loginVM.Password,
                           


                        };
                        _context.LoginModel.Add(inuser);

                    }
                    _context.SaveChanges();
                    if (acctokens != "")
                    {
                        return Ok(new AuthenticatedResponse { Token = acctokens, RefreshToken = refreshToken, Role = role });
                    }
               
                
                }

                }
                return Unauthorized(new { msg="Invalid user name or Password"});
            }
        }

        [HttpGet("alluser")]
        public    List<Vendor> GetAll()
        {
            return _context.Vendors.FromSqlRaw(" select * from vwUser").ToList();
            
        }
        [HttpGet]
        [Route("GetAllUsersWithRoles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            //string query = "SELECT r.Name as roleName, ur.roleId, u.Id as userId   FROM dbo.AspNetUserRoles AS ur INNER JOIN dbo.AspNetUsers AS u ON ur.UserId = u.Id INNER JOIN dbo.AspNetRoles AS r ON ur.RoleId = r.Id ";
            string query = "allUser";
            try
            {
                ICollection<object> usersWithRoles = new List<object>();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.StoredProcedure;

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usersWithRoles.Add(new
                            {
                                
                                Name = reader.GetFieldValueAsync<string>(0).Result,
                                userId = reader.GetFieldValueAsync<string>(1).Result,
                                 
                            });
                        }
                    }
                }
                return StatusCode(200, usersWithRoles); // Get all users   
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        } 
    }
}
