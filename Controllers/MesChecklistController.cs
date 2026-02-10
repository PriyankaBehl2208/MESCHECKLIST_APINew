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
                var messages = await _MESDAL.VALIDATE_USER(Obj.UserName, Obj.Passward);

                if (messages != null && messages.Rows.Count > 0 && messages.Rows[0][0].ToString() == "Valid User")
                {
                    token = CreateToken(Obj.UserName);

                    // Example: extract other values from your DataTable
                    var StageID = messages.Rows[0]["Stage_ID"].ToString();

                    var Departmentval = messages.Rows[0]["Department"].ToString();
                    return Ok(new
                    {
                        statusCode = UDStatusCodes.OK,
                        StageID = StageID,
                        token = token,
                        Department = Departmentval

                    });
                }

                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = "Invalid User" });
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

        [Authorize]
        [HttpPost("GET_ENGINEVALUE")]
    
        public async Task<IActionResult> GET_ENGINEVALUE([FromBody] MES_PREPDI_ENGINE_PIN objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GET_ENGINEVALUE(objUserModel.Pin);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]

        [HttpPost("GETMAIN_LIST")]
        public async Task<IActionResult> GETMAIN_LIST([FromBody] MES_PREPDI_GETMAIN_LIST objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GETMAIN_LIST(objUserModel.Department, objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] MES_PREPDI_UploadImage fileModel)
        {
            if (fileModel.File == null || fileModel.File.Length == 0)
                return BadRequest("No file uploaded.");
            var folderName = DateTime.Now.ToString("yyyy-MM");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);


            // Get current month folder name (e.g., "2025-06")

            // var basePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", folderName);

            var fileName_Created = DateTime.Now.ToString("ddMMyyyy_HHmmss_") + fileModel.Engineno + "_" + fileModel.MASTER_ID + ".png";
            var filePath = Path.Combine(folderPath, fileName_Created);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileModel.File.CopyToAsync(stream);
            }

            var messages = await _MESDAL.save_Image(fileModel.Engineno, fileModel.MASTER_ID, fileName_Created, folderName);
            return Ok(new { statusCode = UDStatusCodes.OK, message = "Image uploaded successfully" });

        }

        [Authorize]

        [HttpPost("UPDATE_REMARKS")]
        public async Task<IActionResult> UPDATE_REMARKS([FromBody] MES_PREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.REMARKS_UPDATED(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }



        [Authorize]

        [HttpPost("UPDATE_REMARKS_STAGE5")]
        public async Task<IActionResult> UPDATE_REMARKS_STAGE5([FromBody] MES_PREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_REMARKS_STAGE5(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }



        



        [Authorize]
        [HttpPost("FINAL_SAVE")]
        public async Task<IActionResult> FINAL_SAVE([FromBody] MES_PREPDI_Engine objUserModel)
        {
            try
            {
                var messages = await _MESDAL.FINALSAVE(objUserModel.Engine_no);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }


        

        [HttpPost("FINAL_SAVE_REVIEW")]
        [Authorize]
        public async Task<IActionResult> FINALFINAL_SAVE_REVIEW_SAVE([FromBody] MES_PREPDI_Engine objUserModel)
        {
            try
            {
                var messages = await _MESDAL.FINAL_SAVE_REVIEW(objUserModel.Engine_no);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        
            [HttpPost("FINAL_SAVE_STAGE5")]
        [Authorize]
        public async Task<IActionResult> FINAL_SAVE_STAGE5([FromBody] MES_PREPDI_Engine objUserModel)
        {
            try
            {
                var messages = await _MESDAL.FINAL_SAVE_STAGE5(objUserModel.Engine_no);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [HttpPost("UPDATE_STATUSOK")]
        [Authorize]
        public async Task<IActionResult> UPDATE_STATUSOK([FromBody] MESPREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_STATUSOK(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }


        [HttpPost("UPDATE_SATUS_STAGE5")]
        [Authorize]
        public async Task<IActionResult> UPDATE_SATUS_STAGE5([FromBody] MESPREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_SATUS_STAGE5(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("STATUSDONE_OK")]
        public async Task<IActionResult> STATUSDONE_OK([FromBody] MESPREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.STATUSDONE_DONEOK(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }


        [HttpPost("GETMAIN_LIST_REVIEW")]
        [Authorize]
        public async Task<IActionResult> GETMAIN_LIST_REVIEW([FromBody] MEPREPDIMODEL_GETMAIN_LIST objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GETMAIN_LIST_REVIEW(objUserModel.Department, objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }



        [HttpPost("INSPECTION_LIST_REVIEW")]
        [Authorize]
        public async Task<IActionResult> INSPECTION_LIST_REVIEW([FromBody] MEPREPDIMODEL_GETMAIN_LISTInspection objUserModel)
        {
            try
            {
                var messages = await _MESDAL.INSPECTION_LIST_REVIEW(objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }




    
         //////////////// STAGE6////////////////////////////////////
       
        [HttpPost("GETMAIN_LIST_MECHANICAL_LIST")]
        [Authorize]
        public async Task<IActionResult> GETMAIN_LIST_MECHANICAL_LIST([FromBody] MEPREPDIMODEL_GETMAIN_LISTInspection objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GETMAIN_LIST_MECHANICAL_LIST(objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }



        [HttpPost("UPDATE_SATUS_STAGE6")]
        [Authorize]
        public async Task<IActionResult> UPDATE_SATUS_STAGE6([FromBody] MESPREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_SATUS_STAGE6(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }




        [Authorize]

        [HttpPost("UPDATE_REMARKS_STAGE6")]
        public async Task<IActionResult> UPDATE_REMARKS_STAGE6([FromBody] MES_PREPDI_UPDATEREMARKS objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_REMARKS_STAGE6(objUserModel.Remarks, objUserModel.Remarks_ID);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }



        [HttpPost("FINAL_SAVE_STAGE6")]
        [Authorize]
        public async Task<IActionResult> FINAL_SAVE_STAGE6([FromBody] MES_PREPDI_Engine objUserModel)
        {
            try
            {
                var messages = await _MESDAL.FINAL_SAVE_STAGE6(objUserModel.Engine_no);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }







        

        [HttpPost("GETMAIN_LIST_HNPC_LIST")]
        [Authorize]
        public async Task<IActionResult> GETMAIN_LIST_HNPC_LIST([FromBody] MEPREPDIMODEL_GETMAIN_LISTInspection objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GETMAIN_LIST_HNPC_LIST(objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }




        [HttpPost("UPDATE_SATUS_STAGE7")]
        [Authorize]
        public async Task<IActionResult> UPDATE_SATUS_STAGE7([FromBody] MESPREPDI_UPDATEREMARKS_Stage7 objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_SATUS_STAGE7(objUserModel.Remarks, objUserModel.Remarks_ID , objUserModel.Department);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }




        [HttpPost("UPDATE_REMARKS_STAGE7")]
        [Authorize]
        public async Task<IActionResult> UPDATE_REMARKS_STAGE7([FromBody] MESPREPDI_UPDATEREMARKS_Stage7 objUserModel)
        {
            try
            {
                var messages = await _MESDAL.UPDATE_REMARKS_STAGE7(objUserModel.Remarks, objUserModel.Remarks_ID, objUserModel.Department);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }





        [HttpPost("FINAL_SAVE_STAGE7")]
        [Authorize]
        public async Task<IActionResult> FINAL_SAVE_STAGE7([FromBody] MES_PREPDI_Engine_Stages objUserModel)
        {
            try
            {
                var messages = await _MESDAL.FINAL_SAVE_STAGE7(objUserModel.PIN_NO , objUserModel.Department);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In UserController at ValidateUser: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }


        




    }
}
