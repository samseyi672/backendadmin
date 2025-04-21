using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Retailbanking.BL.IServices;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Retailbanking.BL.Services;
using System.Collections.Generic;

namespace PrimeAppAdmin.Controllers
{

    [Authorize]
    //[EnableCors("AllowSpecificOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class StaffUserController : ControllerBase
    {

        private readonly IGeneric _genServ;
        private readonly IAuthentication _authServ;
        private readonly IStaffUserService _staffUserService;
        private readonly IStaffServiceDbOperationFilter _staffServiceDbOperationFilter;

        public StaffUserController(IStaffServiceDbOperationFilter staffServiceDbOperationFilter, IGeneric genServ, IAuthentication authServ, IStaffUserService staffUserService)
        {
            _genServ = genServ;
            _authServ = authServ;
            _staffUserService = staffUserService;
            _staffServiceDbOperationFilter = staffServiceDbOperationFilter;
        }

        /// <summary>
        /// Staff Login
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<GenericResponse> LoginStaff([FromBody] StaffLoginRequest loginRequest)
        {
            var response  = await _staffUserService.LoginStaff(loginRequest.UserName,loginRequest.Password);
            return response;
        }

        /// <summary>
        /// Staff get all users from Ad
        /// </summary>
        // [Authorize(Roles = "admin")]
        //[AllowAnonymous]
       //[Authorize(Roles = "Admin", Policy = "CanEditUsersPolicy")]
        [HttpGet("GetAllStaff")]
        public GenericResponse GetAllusers([FromQuery] int page,[FromQuery] int size)
        {
            var response = _staffUserService.GetAllUsers(page,size);
            return response;
        }


         [AllowAnonymous]
         [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SendMailAllToAuthorizerOrAdmin")]
        public async Task<GenericResponse2> SendMailAllToAuthorizerOrAdmin(CustomerDataAtInitiationAndApproval customerDataAtInitiationAndApproval)
        {
            _genServ.LogRequestResponse("GetAuthorizerEmailsAsync", JsonConvert.SerializeObject(customerDataAtInitiationAndApproval),"");
            var emailList = await _staffServiceDbOperationFilter.GetAuthorizerEmailsAsync(customerDataAtInitiationAndApproval);      
            return new GenericResponse2() { Response=EnumResponse.Successful,Success=true};
        }

        /// <summary>
        /// Search users from Ad
        /// </summary>
        //[Authorize(Roles = "admin")]
        //[AllowAnonymous]
        [HttpGet("SearchStaffUsers/{SearchStaff}")]
        public GenericResponse SearchStaffUsers(string SearchStaff)
        {
            var response = _staffUserService.SearchStaffUsers(SearchStaff);
            return response;
        }

        /// <summary>
        /// Profile Staff or Update Staff role and permissions
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost("ProfileStaff")]
        public async Task<GenericResponse> ProfileStaff(ProfileStaff profileStaff)
        {
            if (profileStaff.FirstName == "string") {
                return new GenericResponse() { Success = true, Response = EnumResponse.WrongDetails };
            }
            if (profileStaff.LastName == "string")
            {
                return new GenericResponse() { Success = true, Response = EnumResponse.WrongDetails };
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ProfileStaff(profileStaff,StaffNameAndRole);
            return response;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("GetRoles")]
        public async Task<GenericResponse> GetRoles()
        {
            var response = await _staffUserService.GetRoles();
            return response;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("GetProfiledStaff")]
        public async Task<GenericResponse> GetProfiledStaff([FromQuery] int page , [FromQuery] int size)
        {
            var response = await _staffUserService.GetProfiledStaff(page,size);
            return response;
        }


        [Authorize(Roles = "admin")]
        [HttpPost("GetProfiledStaffWithAuthorities")]
        public async Task<GenericResponse> GetProfiledStaffWithAuthorities([FromQuery] int page, [FromQuery] int size)
        {
            var response = await _staffUserService.GetProfiledStaffWithAuthorities(page, size);
            return response;
        }

        /// <summary>
        /// staff profile inititiation.
        /// pass the staffid to actiontopass.
        /// these are the actions you can pass: 
        /// delete - staff profile
        /// permission-Initiatestaffdelete
        /// </summary>
        [Authorize(Policy = "Initiatestaffdelete")]
        [HttpPost("InitiateStaffDelete/{staffidtoaction}/{actiontopass}")]
        public async Task<GenericResponse> InitiationDeleteActionOnStaffProfile( int staffidtoaction, string actiontopass)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.InitiationDeleteActionOnStaffProfile(staffidtoaction, actiontopass, StaffNameAndRole);
            return response;
        }

        /// <summary>
        /// Get Pending Action on Staff Profile for profile delete only. 
        /// pls keep the id of the actiontopass for later use.
        /// this will also show staff that will perform the actiontopass.
        /// this show the pending delete
        /// </summary>
        [HttpGet("GetPendingDeleteActionOnStaffProfile")]
        public async Task<GenericResponse> GetPendingActionOnStaffProfile()
        {
            var response = await _staffUserService.GetPendingDeleteActionOnStaffProfile();
            return response;
        }

        /// <summary> 
        /// get get pending actiontopass on staff profile count
        /// </summary>
        [HttpGet("GetPendingActionOnStaffProfileCount")]
        public async Task<GenericResponse> GetPendingActionOnStaffProfileCount()
        {
            var response = await _staffUserService.GetPendingActionOnStaffProfileCount();
            return response;
        }

        /// <summary>
        /// Get Pending Profiled to be approved. 
        /// </summary>
        [HttpGet("GetPendingProfiledStaff")]
        public async Task<GenericResponse> GetPendingProfiledStaff()
        {
            var response = await _staffUserService.GetPendingProfiledStaff();
            return response;
        }

        /// <summary>
        /// Get other Pending task to be approved or deny on kyc. 
        /// </summary>
        [HttpGet("GetOtherPendingTaskKyc")]
        public async Task<GenericResponse> GetOtherPendingTask()
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            var response = await _staffUserService.GetOtherPendingTaskKyc(host);
            return response;
        }

        /// <summary>
        /// Get CustomerActivationOrDeactivation  Pending task to be approved or deny on kyc. 
        /// </summary>
        [HttpGet("GetPendingCustomerActivationOrDeactivationTask")]
        public async Task<GenericResponse> GetPendingCustomerActivationOrDeactivationTask()
        {
            var response = await _staffUserService.GetPendingCustomerActivationOrDeactivationTask();
            return response;
        }


        /// <summary>
        /// Get GetCustomerPending task to be approved or deny . 
        /// </summary>
        [HttpGet("GetCustomerIndemnityPendingTask")]
        public async Task<GenericResponse> GetcustomerIndemnityPendingTask()
        {
            var response = await _staffUserService.GetcustomerIndemnityPendingTask();
            return response;
        }

        /// <summary>
        /// Get GetCustomerPending task to be approved or deny. 
        /// </summary>
        [HttpGet("GetAccountIndemnityPendingTask")]
        public async Task<GenericResponse> GetAccountIndemnityPendingTask()
        {
            var response = await _staffUserService.GetAccountIndemnityPendingTask();
            return response;
        }

        /// <summary>
        /// Get indemnity request from mobile app
        /// set indemnitytype=customer for customer indemnity
        /// set indemnitytype=account for account indenity
        /// </summary>
        [HttpGet("GetIndemnityRequest")]
        public async Task<GenericResponse2> GetIndemnityRequest([FromQuery] int page, [FromQuery] int size,[FromQuery] string indemnitytype)
        {
            return await _staffUserService.GetIndemnityRequest(page, size,indemnitytype);
        }

        /// <summary>
        /// Get approved indemnity
        /// set indemnitytype=customer for customer indemnity
        /// set indemnitytype=account for account indenity
        /// </summary>
        [HttpGet("GetApprovedIndemnityRequest")]
        public async Task<GenericResponse2> GetApprovedIndemnityRequest([FromQuery] int page, [FromQuery] int size, [FromQuery] string indemnitytype)
        {
            return await _staffUserService.GetApprovedIndemnityRequest(page, size, indemnitytype);
        }

        /// <summary>
        /// Get count of Pending Profiled to be approved
        /// </summary>
        [HttpGet("GetPendingProfiledStaffCount")]
        public async Task<GenericResponse> GetPendingProfiledStaffCount()
        {
            var response = await _staffUserService.GetPendingProfiledStaffCount();
            return response;
        }

        /// <summary>
        /// permission-approvedeletestaff
        /// delete staff on exit
        /// pass the id of the actiontopass which I supplied on GetPendingDeleteActionOnStaffProfile and pass it here .
        /// </summary>
        [Authorize(Policy = "approvedeletestaff")]
        [HttpPost("DeleteStaffProfile/{actionid}/{approveordeny}")]
        public async Task<GenericResponse> DeleteStaffProfile(int actionid,string approveordeny)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.DeleteStaffProfile(actionid, StaffNameAndRole,approveordeny);
            return response;
        }

        /// <summary>
        /// approveordeny can either be approve or deny
        /// permission-approvestaffprofile
        /// pass the id of the actiontopass which I supplied on GetPendingDeleteActionOnStaffProfile and pass it here .
        /// </summary>
        [Authorize(Policy = "approvestaffprofile")]
        [HttpPost("ApproveStaffProfile/{actionid}/{approveordeny}")]
        public async Task<GenericResponse> ApproveStaffProfile(int actionid,string approveordeny)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ApproveStaffProfile(actionid,StaffNameAndRole,approveordeny);
            return response;
        }

        /// <summary>
        /// approveordeny can either be approve or deny
        /// permission-approvekyc
        /// pass the id of the actiontopass which I supplied on GetOtherPendingTaskKyc and pass it here.
        /// shortdescription is needed for kyc otherwise just pass ""
        /// typeofdocument for passport use passport
        /// typeofdocument for idcard use idcard
        /// typeofdocument for signature use signature
        /// typeofdocument for utilitybill use utilitybill
        /// </summary>
        [Authorize(Policy = "approvekyc")]
        [HttpPost("ApproveKyc/{actionid}/{approveordeny}/{shortdescription}/{typeofdocument}")]
        public async Task<GenericResponse> ApproveTask(int actionid, string approveordeny,string shortdescription,string typeofdocument)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            //var response = await _staffUserService.ApproveTask(actionid, StaffNameAndRole, approveordeny,shortdescription);
            var response = await _staffUserService.ApproveKycTask(actionid, StaffNameAndRole, approveordeny, shortdescription,typeofdocument);
            return response;
        }


        /// <summary>
        /// count of status pending initiation and approval
        /// </summary>
        [HttpPost("CountOfPendingKycInitiationAndApproval")]
        public async Task<GenericResponse2> CountOfPendingKycInitiationAndApproval()
        {
            var response = await _staffUserService.CountOfPendingKycInitiationAndApproval();
            return response;
        }

        /// <summary>
        /// Get Capped Transaction  limit
        /// </summary>
        [HttpPost("GetCappedTransactionLimit")]
        public async Task<GenericResponse2> GetCappedTransactionLimit()
        {
            var response = await _staffUserService.GetCappedTransactionLimit();
            return response;
        }

        /// <summary>
        /// Set and Initiate Transaction Capped Limit at the Same time
        /// </summary>
        [Authorize(Policy = "initiatetranscappedlimit")]
        [HttpPost("InitiateTransactionCappedLimit")]
        public async Task<GenericResponse2> InitiateTransactionCappedLimit(TransactionCappedLimit setTransactionCappedLimit)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.InitiateTransactionCappedLimit("initiatetranscappedlimit", setTransactionCappedLimit,StaffNameAndRole);
            return response;
        }

        /// <summary>
        /// Get pending Transaction Capped Limit
        /// </summary>
        [HttpPost("GetPendingTransactionCappedLimit")]
        public async Task<GenericResponse2> GetPendingTransactionCappedLimit()
        {
            var response = await _staffUserService.GetPendingTransactionCappedLimit();
            return response;
        }

        /// <summary>
        /// Approve Transaction Capped Limit
        /// </summary>
        [Authorize(Policy = "approvetranscappedlimit")]
        [HttpPost("ApproveTransactionCappedLimit/{actionid}/{approveordeny}")]
        public async Task<GenericResponse2> ApproveTransactionCappedLimit(int actionid,string approveordeny)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ApproveTransactionCappedLimit(approveordeny,null,actionid, StaffNameAndRole);
            return response;
        }


        /// <summary>
        /// approveordeny can either be approve or deny
        /// permission-customerdeactivation,customeractivation
        /// pass the id of the actiontopass which I supplied on GetOtherPendingTaskKyc and pass it here.
        /// shortdescription is need for kyc otherwise just pass ""
        /// </summary>
       // [Authorize(Policy = "customerdeactivation")]
       // [Authorize(Policy = "customeractivation")]
        [HttpPost("ApproveCustomerActivationOrDeactivation/{actionid}/{approveordeny}/{shortdescription}")]
        public async Task<GenericResponse> ApproveCustomerActivationOrDeactivation(int actionid, string approveordeny, string shortdescription)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ApproveTask(actionid, StaffNameAndRole, approveordeny, shortdescription);
            return response;
        }

        /// <summary>
        /// approveordeny can either be approve or deny
        /// permission-acceptlimit,rejectlimit,customeractivation
        /// pass the id of the actiontopass which I supplied on GetOtherPendingTaskKyc and pass it here.
        /// shortdescription is need for kyc otherwise just pass ""
        /// </summary>
        [Authorize(Policy = "acceptcustomerindemnity")]
        [Authorize(Policy = "rejectcustomerindemnity")]
        [HttpPost("ApproveCustomerIndemnity/{actionid}/{approveordeny}/{shortdescription}")]
        public async Task<GenericResponse> ApproveCustomerIndemnity(int actionid, string approveordeny, string shortdescription)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ApproveTask(actionid, StaffNameAndRole, approveordeny, shortdescription);
            return response;
        }

        /// <summary>
        /// approveordeny can either be approve or deny
        /// permission-acceptaccountindemnitylimit,rejectaccountindemnitylimit
        /// pass the id of the actiontopass which I supplied on GetOtherPendingTaskKyc and pass it here.
        /// shortdescription is need for kyc otherwise just pass ""
        /// </summary>
        [Authorize(Policy = "acceptaccountindemnitylimit")]
        [Authorize(Policy = "rejectaccountindemnitylimit")]
        [HttpPost("ApproveAccountIndemnity/{actionid}/{approveordeny}/{shortdescription}/{AccountNumber}")]
        public async Task<GenericResponse> ApproveAccountIndemnity(int actionid, string approveordeny, string shortdescription,string AccountNumber)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _staffUserService.ApproveTask(actionid, StaffNameAndRole, approveordeny, shortdescription,AccountNumber);
            return response;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetStaffPermissions/{staffid}")]
        public async Task<GenericResponse> GetStaffPermissions(int staffid)
        {
            var response = await _staffUserService.GetStaffPermissions(staffid);
            return response;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetPermissions/{roleName}")]
        public async Task<GenericResponse> GetPermissions(string roleName)
        {
            var response = await _staffUserService.GetPermissions(roleName);
            return response;
        }

        [Authorize(Roles ="admin")]
        [HttpGet("GetRolesAndPermissions/{email}")]
        public async Task<GenericResponse> GetRolesAndPermissions(string email)
        {
            var response = await _staffUserService.GetRolesAndPermissions(email);
            return response;
        }

    }
}































































































































































