using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailbanking.BL.IServices;
using Retailbanking.BL.Services;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;

namespace PrimeAppAdmin.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PinManagementController : ControllerBase
    {
        private readonly IPinService _pinService;
        private readonly IPinManagementService _pinManagementService;

        public PinManagementController(IPinService pinService,IPinManagementService pinManagementService)
        {
            _pinService = pinService;
            _pinManagementService = pinManagementService;
        }

         [HttpGet("GetForgotPinRequest")]
        public async Task<GenericResponse2> GetForgotPinRequest([FromQuery]int page ,[FromQuery]int size)
        {
            return await _pinService.GetForgotPinRequest(page, size);
        }

        /// <summary>
        /// action= initiatepinapproval
        /// </summary>
        [Authorize(Policy = "initiatepinapproval")]
        [HttpPost("InitiatePinApproval/{action}/{username}")]
        public async Task<GenericResponse2> InitiatePinApproval(string action,string username)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            return await _pinManagementService.InitiatePinApproval(action,username,StaffNameAndRole);
        }

        /// <summary>
        /// approveordeny can be either approve or deny
        /// </summary>
        [Authorize(Policy = "pinapproval")]
        [HttpPost("ApprovePinChange")]
        public async Task<GenericResponse2> PinApproval([FromBody] PinApproval pinApproval)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            pinApproval.StaffNameAndRole = StaffNameAndRole;
            return await _pinManagementService.PinApproval(pinApproval);
        }

        /// <summary>
        /// Get List Of InitiatedPinApproval
        /// </summary>
        [HttpPost("GetListOfInitiatedPinApproval")]
        public async Task<GenericResponse2> GetListOfInitiatedPinApproval(int page,int size)
        {
            return await _pinManagementService.GetListOfInitiatedPinApproval(page,size);
        }
    }
}































































































































































































































