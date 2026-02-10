using MESCHECKLIST.DataAccess;
using MESCHECKLIST.Model;
using MESCHECKLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MESCHECKLIST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Stage1_2_3Controller : ControllerBase
    {
        private readonly ILogger<Stage1_2_3Controller> _logger;
        private readonly Stag1_2_3DAL _MESDAL;
        private readonly IMemoryCache _memoryCache;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly IDataProtector _protector;
        IConfigurationSection appSettings;


        public Stage1_2_3Controller(ILogger<Stage1_2_3Controller> logger, Stag1_2_3DAL MESCHECKDAL, IMemoryCache memoryCache, IDataProtectionProvider dataProtectionProvider)
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

        [HttpPost("GetHistoryCard")]
        [Authorize]
        public async Task<IActionResult> GetHistoryCard([FromBody] CraneMaster objCraneMaster)
        {
            try
            {
                var messages = await _MESDAL.GetHistoryCard(objCraneMaster.engineNo,objCraneMaster.model,objCraneMaster.chassisNo);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In Stage1_2_3Controller at GetHistoryCard: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
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
                _logger.LogError("In Stage1_2_3Controller at GET_ENGINEVALUE: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("SaveHistoryCard_Make")]

        public async Task<IActionResult> SaveHistoryCard_Make([FromBody] SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                var messages = await _MESDAL.SaveHistoryCard_Make(objSaveHistoryCard_Make);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In Stage1_2_3Controller at SaveHistoryCard_Make: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("SaveHistoryCard_Aging")]

        public async Task<IActionResult> SaveHistoryCard_Aging([FromBody] SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                var messages = await _MESDAL.SaveHistoryCard_Aging(objSaveHistoryCard_Make);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In Stage1_2_3Controller at SaveHistoryCard_Aging: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }


        [Authorize]
        [HttpPost("SaveHistoryCard_Serial")]

        public async Task<IActionResult> SaveHistoryCard_Serial([FromBody] SaveHistoryCard_Make objSaveHistoryCard_Make)
        {
            try
            {
                var messages = await _MESDAL.SaveHistoryCard_Serial(objSaveHistoryCard_Make);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In Stage1_2_3Controller at SaveHistoryCard_Serial: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

        [Authorize]

        [HttpPost("GETMAIN_LISTStage1")]
        public async Task<IActionResult> GETMAIN_LISTStage1([FromBody] MES_PREPDI_GETMAIN_LIST objUserModel)
        {
            try
            {
                var messages = await _MESDAL.GETMAIN_LISTStage1(objUserModel.Modelno, objUserModel.Pin, objUserModel.Engineno, objUserModel.Name);
                return Ok(new { statusCode = UDStatusCodes.OK, message = messages });
            }
            catch (Exception ex)
            {
                _logger.LogError("In Stage1_2_3Controller at GETMAIN_LISTStage1: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }

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
                _logger.LogError("In Stage1_2_3Controller at UPDATE_REMARKS: " + ex.Message);
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
                _logger.LogError("In Stage1_2_3Controller at FINAL_SAVE: " + ex.Message);
                return Ok(new { statusCode = UDStatusCodes.BadRequest, message = ex.Message });
            }
        }
    }
}
