using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MESCHECKLIST.DataAccess;
using MESCHECKLIST.Model;
using MESCHECKLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace MESCHECKLIST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesChecklistController : ControllerBase
    {

        private readonly ILogger<MesChecklistController> _logger;
        private readonly MESCHECLISTDAL _MESDAL;
        private readonly IMemoryCache _memoryCache;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly IDataProtector _protector;
        IConfigurationSection appSettings;


        public MesChecklistController(ILogger<MesChecklistController> logger, MESCHECLISTDAL MESCHECKDAL, IMemoryCache memoryCache, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _MESDAL = MESCHECKDAL;
            _memoryCache = memoryCache;
            var _configuration = new ConfigurationBuilder()
                                              .AddJsonFile("appSettings.Development.json")
                                              .Build();
            appSettings = _configuration.GetSection("AppSettings");
            var secretKey = appSettings["EncryptionSecretKey"];
            _protector = dataProtectionProvider.CreateProtector(secretKey);
        }




        [HttpPost("VALIDATE_USER")]
        public async Task<IActionResult> VALIDATE_USER([FromBody] MESVALIDATE_USER Obj)
        {
            try
            {

                string token = "";
                var messages = await _MESDAL.VALIDATE_USER();
                if (messages != null && messages.Rows.Count > 0)
                {
                    token = CreateToken(Obj.UserName);

                }
                return Ok(new { statusCode = UDStatusCodes.OK, message = token });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }




        private string CreateToken(string empcode)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, empcode), // Fixed: Replaced 'username' with 'empcode'
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: appSettings["Issuer"],
                audience: appSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
