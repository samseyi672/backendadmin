using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using Retailbanking.BL.IServices;
using Retailbanking.BL.Services;

namespace PrimeAppAdmin.Controllers
{


    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class KyCController : ControllerBase
    {


        private readonly IGeneric _genServ;
        private readonly IAuthentication _authServ;
        private readonly FolderPaths _folderPaths;
        private readonly IStaffUserService _staffUserService;
        private readonly IMobileUserService _mobileUserService;

        public KyCController(IOptions<FolderPaths> folderPaths, IMobileUserService mobileUserService, IGeneric genServ, IAuthentication authServ, IStaffUserService staffUserService)
        {
            _genServ = genServ;
            _authServ = authServ;
            _folderPaths = folderPaths.Value;
            _staffUserService = staffUserService;
            _mobileUserService = mobileUserService;
        }


        /// <summary>
        ///  set image status to false
        ///  when it is no longer needed.
        ///  permission- acceptkyc,rejectkyc
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "acceptkyc,rejectkyc")]
        [HttpPost("KycSignatureAcceptance")]
        public async Task<GenericResponse> KycSignatureAcceptance(KycSignature kycStatus)
        {
            var resp = await _authServ.KycSignatureAcceptance("", kycStatus);
            Console.WriteLine("KycSignatureAcceptance ...." + resp);
            _genServ.LogRequestResponse("KycSignatureAcceptance ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  View user kyc status 
        /// </summary>
       // [Authorize(Roles = "admin", Policy = "viewkyc")]
        [HttpGet("CheckKyC/{UserName}")]
        public async Task<GenericResponse> CheckKyC(string UserName)
        {
            var response = await _mobileUserService.CheckAUserKyc(UserName);
            return response;
        }

        /// <summary>
        ///  Get employmentinfo
        /// </summary>
       // [Authorize(Roles = "admin", Policy = "viewkyc")]
        [HttpGet("KycEmploymentInfo/{UserName}")]
        public async Task<GenericResponse> GetAndViewUserKyc(string UserName)
        {
            var response = await _mobileUserService.GetUserKycEmploymentInfo(UserName);
            return response;
        }

        /// <summary>
        ///  Get kyc nextofkininfo
        /// </summary>
        [HttpGet("KycNextOfKinInfo/{UserName}")]
        public async Task<GenericResponse> GetKycNextOfKinInfo(string UserName)
        {
            var response = await _mobileUserService.GetKycNextOfKinInfo(UserName);
            return response;
        }

        /// <summary>
        ///  set image status to false
        ///  when it is no longer needed.
        ///  permission- acceptkyc,rejectkyc
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "acceptkyc,rejectkyc")]
        [HttpPost("KycIdCardAcceptance")]
        public async Task<GenericResponse> KycIdCardAcceptance(KycidCard kycStatus)
        {
            var resp = await _authServ.KycIdCardAcceptance("", kycStatus);
            Console.WriteLine("KycAcceptance ...." + resp);
            _genServ.LogRequestResponse("KycIdCardAcceptance ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }


        /// <summary>
        ///  set image status to false
        ///  when it is no longer needed.
        ///  permission- acceptkyc,rejectkyc
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "acceptkyc,rejectkyc")]
        [HttpPost("KycPassportAcceptance")]
        public async Task<GenericResponse> KycPassportAcceptance(KycPassport kycStatus)
        {
            var resp = await _authServ.KycPassportAcceptance("", kycStatus);
            Console.WriteLine("KycAcceptance ...." + resp);
            _genServ.LogRequestResponse("KycPassportAcceptance ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  set image status to false
        ///  when it is no longer needed.
        ///  permission- acceptkyc,rejectkyc
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "acceptkyc")]
        [Authorize(Policy = "rejectkyc")]
        [HttpPost("KycUtlityBillAcceptance")]
        public async Task<GenericResponse> KycUtlityBillAcceptance(KycUtlityBill kycStatus)
        {
            var resp = await _authServ.KycUtlityBillAcceptance("", kycStatus);
            Console.WriteLine("KycAcceptance ...." + resp);
            _genServ.LogRequestResponse("KycAcceptance ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        /// Initiation kyc acceptance or rejection
        ///  permission- acceptkyc,rejectkyc
        ///  for utilitybill,passport,idcard,signature
        ///  set status as accept or reject
        /// </summary>
         [Authorize(Policy = "acceptkyc")]
         [Authorize(Policy = "rejectkyc")]
        [HttpPost("KycAcceptance")]
        public async Task<GenericResponse> KycAcceptance(Kyc kycStatus)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            return await _authServ.KycAcceptance(kycStatus, kycStatus.type,StaffNameAndRole);
        }

        // [Authorize(Roles = "admin", Policy = "viewkyc")]
        [HttpGet("KycStatus/{Username}")]
        public async Task<GenericResponse> KycStatus(string Username)
        {
            var resp = await _authServ.KycStatus(Username);
            Console.WriteLine("KycAcceptance ...." + resp);
            _genServ.LogRequestResponse("KycStatus ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }
    }
}
