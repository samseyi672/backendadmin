using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz.Impl.Triggers;
using Retailbanking.BL.IServices;
using Retailbanking.BL.Services;
using Retailbanking.Common.CustomObj;
using Retailbanking.Common.DbObj;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PrimeAppAdmin.Controllers
{
 
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PrimeUserController : ControllerBase
    {

        private readonly IGeneric _genServ;
        private readonly IAuthentication _authServ;
        private readonly FolderPaths _folderPaths;
        private readonly IStaffUserService _staffUserService;
        private readonly IMobileUserService _mobileUserService;

        public PrimeUserController(IOptions<FolderPaths> folderPaths, IMobileUserService mobileUserService,IGeneric genServ, IAuthentication authServ, IStaffUserService staffUserService)
        {
            _genServ = genServ;
            _authServ = authServ;
            _folderPaths = folderPaths.Value;
            _staffUserService = staffUserService;
            _mobileUserService = mobileUserService;
        }

        /// <summary>
        /// Get All Users Registered on Mobile
        /// </summary>
        [HttpGet("GetPrimeUsers")]
        public async Task<GenericResponse> GetAllMobileUsers([FromQuery] int page,[FromQuery] int size)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            _genServ.LogRequestResponse("GetAllMobileUsers", "host",host);
            var response = await _mobileUserService.GetPrimeUsers(page, size,host);
            return response;
        }

        /// <summary>
        /// Search user by name
        /// </summary>
        [HttpGet("SearchPrimeUserByName/{SearchTerm}")]
        public async Task<GenericResponse> SearchUserByName(string SearchTerm)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.SearchUserByName(1,10, host,SearchTerm);
            return response;
        }


        /// <summary>
        /// Search user by bvn
        /// </summary>
        //[Authorize(Roles = "Admin", Policy = "viewuser")]
        [HttpGet("SearchPrimeUserByBvn/{BvnSearch}")]
        public async Task<GenericResponse> SearchUserByBvn(string BvnSearch)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.SearchUserByBvn(1,10,host, BvnSearch);
            return response;
        }

        /// <summary>
        /// Get account details with account tier
        /// </summary>
        [HttpGet("AccountDetailsAndAccountTier/{Username}")]
        public async Task<GenericResponse> GetUserAccountDetailsWithKycLevel(string Username)
        {
            var response = await _mobileUserService.GetUserAccountDetailsWithKycLevel(Username);
            return response;
        }
        
        /// <summary>
        ///  This is use to view a user kyc documents if it is permitted
        /// </summary>
        [HttpGet("KycDocument/{UserName}")]
        public async Task<GenericResponse> GetAndViewUserKycDocument(string UserName)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            //string picpath = host 
            Console.WriteLine("host "+host);
            var response = await _mobileUserService.GetUserKycDocument(UserName,host);
            return response;
        }


        /// <summary>
        ///  View a file on server
        /// </summary>
        [AllowAnonymous]
        [HttpGet("BrowserView/{filename}")]
        public IActionResult BrowserView(string filename)
        {
            var filePath = Path.Combine(_folderPaths.Uploads, filename);
            Console.WriteLine("_folderPaths.Uploads " + _folderPaths.Uploads);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }   
            //var mimeType = MimeTypes.GetMimeType(filename);
            var mimeType = MimeTypesMap.GetMimeType(filePath);
            Console.WriteLine("mimeType " + mimeType);
           // _logger.LogInformation("browser filepath "+filePath);
            return PhysicalFile(filePath, mimeType);
        }

        /// <summary>
        ///  View a file on server
        /// </summary>
        [AllowAnonymous]
        [HttpGet("AdvertBrowserView/{filename}")]
        public IActionResult AdvertBrowserView(string filename)
        {
            var filePath = Path.Combine(_folderPaths.AdvertImage, filename);
            Console.WriteLine("_folderPaths.Uploads " + _folderPaths.Uploads);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            //var mimeType = MimeTypes.GetMimeType(filename);
            var mimeType = MimeTypesMap.GetMimeType(filePath);
            Console.WriteLine("mimeType " + mimeType);
            // _logger.LogInformation("browser filepath "+filePath);
            return PhysicalFile(filePath, mimeType);
        }

        /// <summary>
        ///  View pdf file 
        /// </summary>
        [AllowAnonymous]
        [HttpGet("FileView/{filename}")]
        public IActionResult FileView(string filename)
        {
            var filePath = Path.Combine(_folderPaths.IndemnityformPath, filename);
            Console.WriteLine("_folderPaths.Uploads " + _folderPaths.Uploads);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var mimeType = MimeTypesMap.GetMimeType(filePath);
            if (mimeType == "application/pdf")
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                // Set Content-Disposition to inline to display it in the browser
                Response.Headers.Add("Content-Disposition", $"inline; filename={filename}");
                return File(fileBytes, mimeType);
            }
            Console.WriteLine("mimeType " + mimeType);
            // _logger.LogInformation("browser filepath "+filePath);
            return PhysicalFile(filePath, mimeType);
        }

        [AllowAnonymous]
        [HttpGet("KycView/{filename}")]
        public IActionResult KycView(string filename)
        {
            var filePath = Path.Combine(_folderPaths.kycUpload, filename);
           // _genServ.LogRequestResponse("_folderPaths.Uploads " + _folderPaths.kycUpload);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var mimeType = MimeTypesMap.GetMimeType(filePath);
           // Console.WriteLine("mimeType " + mimeType);
            return PhysicalFile(filePath, mimeType);
        }

        [AllowAnonymous]
        [HttpGet("KycView2/{filename}")]
        public IActionResult KycView2(string filename)
        {
            var filePath = Path.Combine(_folderPaths.PicFileUploadPath, filename);
            // _genServ.LogRequestResponse("_folderPaths.Uploads " + _folderPaths.kycUpload);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var mimeType = MimeTypesMap.GetMimeType(filePath);
            // Console.WriteLine("mimeType " + mimeType);
            return PhysicalFile(filePath, mimeType);
        }

        /// <summary>
        ///  Activate a user
        ///  permission-initiatecustomeractivation
        /// </summary>
        [Authorize(Policy = "initiatecustomeractivation")]
        [HttpPost("InitiateActivateACustomer/{UserName}")]
        public async Task<GenericResponse> InitiateActivateACustomer(string UserName)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.InitiateActivateACustomer(UserName, "initiatecustomeractivation", StaffNameAndRole);
            return response;
        }

        /// <summary>
        ///  Deactivate a user
        ///  permission-initiatecustomerdeactivation
        /// </summary>
        [Authorize(Policy = "initiatecustomerdeactivation")]
        [HttpPost("InitiateDeactivateCustomer/{UserName}")]
        public async Task<GenericResponse> InitiateDeactivateCustomer(string UserName)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.InitiateDeactivateCustomer(UserName, "initiatecustomerdeactivation", StaffNameAndRole);
            return response;
        }

        /// <summary>
        ///  Get Count of Transaction for the Month
        /// </summary>
        [HttpPost("GetCountOfTransactionFortheMonth")]
        public async Task<GenericResponse> GetCountOfTransactionFortheMonth()
        {
            var response = await _mobileUserService.GetCountOfTransactionFortheMonth();
            return response;
        }


        /// <summary>
        ///  Upgrade Customer Account
        ///  permission-upgradeaccount
        ///  dont use this anymore
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "upgradeaccount")]
        [HttpPost("UpgradeAccount/{UserName}")]
        public async Task<GenericResponse> UpgradeAccount(string UserName,[FromBody] UpgradeAccountNo upgradeAccountNo)
        {
           // var response = await _mobileUserService.UpgradeAUserAccount(UserName, upgradeAccountNo);
            return new GenericResponse() { Response=EnumResponse.NotSuccessful};
        }

        /// <summary>
        /// approveordeny can either be approve or deny
        /// shortdescription is needed for kyc otherwise just pass empty string
        /// permission -approveaccountupgrade
        /// </summary>
         [Authorize(Policy = "approveaccountupgrade")]
        [HttpPost("ApproveAccountUpgrade")]
        public async Task<GenericResponse> ApproveAccountUpgrade(ApproveAccountUpgrade approveAccountUpgrade)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.ApproveAccountUpgrade(approveAccountUpgrade.UserName,approveAccountUpgrade.upgradeAccountNo, approveAccountUpgrade.actionid, StaffNameAndRole, approveAccountUpgrade.approveordeny, approveAccountUpgrade.shortdescription);
            return response;
        }

        /// <summary>
        ///  Upgrade Customer Account
        ///  permission-Initiateupgradeaccount
        /// </summary>
        [Authorize(Policy = "initiateupgradeaccount")]
        [HttpPost("Initiateupgradeaccount/{UserName}/{AccountNumber}/{AccountTier}")]
        public async Task<GenericResponse> Initiateupgradeaccount(string UserName,string AccountNumber,string AccountTier)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.Initiateupgradeaccount(UserName,AccountTier, AccountNumber, "Initiateupgradeaccount", StaffNameAndRole);
            return response;
        }


        /// <summary>
        ///  Get list of pending account to be upgraded
        /// </summary>
        [HttpPost("GetPendingListOfAccountTobeUpgraded")]
        public async Task<GenericResponse> GetPendingAccountTobeUpgraded()
        {
            var response = await _mobileUserService.GetPendingListOfAccountTobeUpgraded();
            return response;
        }

        /// <summary>
        ///  Add Advert/Promo Image/pictures
        ///  role-admin,Initiator
        /// </summary>
        [Authorize(Roles = "admin,Initiator")]
        [HttpPost("AddAdvertImageOrPicture/{Active}")]
        public async Task<GenericResponse> AddAdvertImageOrPictures(bool Active,IFormFile image)
        {
            var response = await _mobileUserService.AddAdvertImageOrPictures(Active,"",image);
            return response;
        }

        /// <summary>
        ///  Get Advert/Promo Image/pictures
        /// </summary>
       // [Authorize(Roles = "Admin", Policy = "viewkyc")]
        [HttpPost("GetAdvertOrPromoImage")]
        public async Task<GenericResponse> GetAdvertImagesorPromoImage([FromQuery] int page , [FromQuery] int size)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.GetAdvertImagesorPromoImage(host,page,size);
            return response;
        }

        /// <summary>
        ///  Delete Advert/Promo Image/pictures
        ///  role-admin,Authorizer
        /// </summary>
        /// <param name="ids">A list of image IDs to delete.[1,2,3]</param>
        /// <returns>A response indicating success or failure.</returns>
        [Authorize(Roles = "admin,Authorizer")]
        [HttpPost("DeleteAdvertImageorPromoImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<GenericResponse> DeleteAdvertImagesorPromoImage([FromBody] List<int> ids)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
           // var response = await _mobileUserService.GetAdvertImagesorPromoImage(host, page, size);
            var response = await _mobileUserService.DeleteAdvertImagesorPromoImage(host,ids);
            return response;
        }


        /// <summary>
        ///  set image status to false
        ///  when it is no longer needed.
        ///  role-admin,Authorizer
        /// </summary>
        [Authorize(Roles = "admin,Authorizer")]
        [HttpPost("UpdateAdvertOrPromoImageStatus")]
        public async Task<GenericResponse> EditAdvertImagesorPromoImage([FromBody] List<ImageUpdate> ids)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.EditAdvertImagesorPromoImage(ids);
            return response;
        }

       // [Authorize(Roles = "admin,Authorizer,Initiator")]
        [HttpGet("GetActiveAndInActiveCustomerFromFinedge")]
        public async Task<GenericResponse2> GetActiveAndInActiveCustomer()
        {
            var resp = await _mobileUserService.GetActiveAndInActiveCustomer() ;
            Console.WriteLine("GetActiveAndInActiveCustomer ...." + resp);
            _genServ.LogRequestResponse("GetActiveAndInActiveCustomer ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  mobile channel only
        ///  To get customers that are not active in order to follow up on them
        /// </summary>
        [HttpGet("GetMobileActiveAndInActiveCustomer")]
        public async Task<GenericResponse2> GetMobileActiveAndInActiveCustomer()
        {
            var resp = await _mobileUserService.GetMobileActiveAndInActiveCustomer();
            Console.WriteLine("GetMobileActiveAndInActiveCustomer ...." + resp);
            _genServ.LogRequestResponse("GetMobileActiveAndInActiveCustomer ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  mobile channel only.
        ///  To get reported transactions
        /// </summary>
        [HttpGet("GetReportedTransactions")]
        public async Task<GenericResponse2> GetTransactionsReported()
        {
            var resp = await _mobileUserService.GetTransactionsReported();
            Console.WriteLine("GetTransactionsReported ...." + resp);
            _genServ.LogRequestResponse("GetTransactionsReported ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  mobile channel only.
        ///  To set reported transactions as fixed
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("FixReportedTransactions/{UserName}/{TransactionRef}")]
        public async Task<GenericResponse2> FixReportedTransactions(string UserName,string TransactionRef)
        {
            var resp = await _mobileUserService.FixReportedTransactions(UserName, TransactionRef);
            Console.WriteLine("FixReportedTransactions ...." + resp);
            _genServ.LogRequestResponse("FixReportedTransactions ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        ///  mobile channel only.
        ///  To update reported transactions  status
        ///  PASS 1 for UNDER_INVESTIGATION for CheckedStatus
        ///  PASS 2 for  RESOLVED for CheckedStatus
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = "fixreportedtransaction")]
        [HttpGet("UpdateReportedTransactionStatus/{UserName}/{TransactionRef}/{Status}")]
        public async Task<GenericResponse2> UpdateReportedTransactionStatus(string UserName, string TransactionRef,int Status)
        {
            var resp = await _mobileUserService.UpdateReportedTransactionStatus(UserName, TransactionRef, Status);
            Console.WriteLine("UpdateReportedTransactionStatus ...." + resp);
            _genServ.LogRequestResponse("UpdateReportedTransactionStatus ...", null, JsonConvert.SerializeObject(resp));
            return resp;
        }

        /// <summary>
        /// Get a user account balance details
        /// </summary>
        [HttpGet("AccountBalance/{Username}")]
        public async Task<GenericResponse> GetAllCustomerAccountBalance(string Username)
        {
            var response = await _mobileUserService.GetCustomerAllAccountBalance(Username);
            return response;
        }

        /// <summary>
        /// Get Mobile trans history.date format(dd-MM-yyyy)
        /// </summary>
        [HttpGet("TransactionHistory/{username}/{page}/{size}/{StartDate}/{EndDate}")]
        public async Task<GenericResponse> GetAUserMobileTrasanctionHistory(string username, int page, int size, string StartDate, string EndDate)
        {
            var response = await _mobileUserService.GetAUserMobileTransactionHistory(page,size,username,StartDate,EndDate);
            return response;
        }

       // [Authorize(Roles = "admin,Authorizer,Initiator,viewer")]
        [HttpGet("CheckIntraBankTransactionstatus/{TransRef}")]
        public async Task<GenericResponse2> GetCustomerTransactionusStatus(string TransRef)
        {
            var response = await _mobileUserService.GetCustomerIntrabankTransactionusStatus(TransRef,"");
            return response;
        }

       // [Authorize(Roles = "admin,Authorizer,Initiator,viewer")]
        [HttpGet("CheckInterBankTransactionstatus/{TransRef}")]
        public async Task<GenericResponse2> GetCustomerInterBankTransactionusStatus(string TransRef)
        {
            var response = await _mobileUserService.GetCustomerInterBankTransactionusStatus(TransRef, "");
            return response;
        }

        /// <summary>
        /// Get pending kyc counter
        /// </summary>
        [HttpGet("GetPendingKycCount")]
        public async Task<GenericResponse2> GetPendingKycCount()
        {
            var response = await _mobileUserService.GetPendingKycCount();
            return response;
        }

        /// <summary>
        /// Get Pending AccountLimitUpdate
        /// </summary>
        [HttpGet("GetPendingAccountLimitUpdateCount")]
        public async Task<GenericResponse2> GetPendingAccountLimitUpdate()
        {
            var response = await _mobileUserService.GetPendingAccountLimitUpdate();
            return response;
        }

        /// <summary>
        /// Get CustomerAccountLimit/CustomerIndemnity details of customer
        /// </summary>
        [HttpGet("GetDetailCustomerAccountLimitUpdate/{username}")]
        public async Task<GenericResponse2> GetCustomerAccountLimitUpdate(string username)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.GetCustomerAccountLimitUpdate(username,host);
            return response;
        }

        /// <summary>
        /// Get Pending CustomerIndemnity username initiated by customer
        /// </summary>
        [HttpGet("GetPendingCustomerAccountLimitUpdate")]
        public async Task<GenericResponse2> GetPendingCustomerAccountLimitUpdate()
        {
            var response = await _mobileUserService.GetPendingCustomerAccountLimitUpdate();
            return response;
        }

        /// <summary>
        ///  Initiate customer indemnitylimit.
        ///  permission-initiatecustomerindemnity.
        /// </summary>
        [Authorize(Policy = "initiatecustomerindemnity")]
        [HttpPost("InitiateCustomerIndemnityLimit/{username}")]
        public async Task<GenericResponse> CustomerAccountIndemnityLimitAcceptance(string username)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.InitiateCustomerIndmnityLimitAcceptance(username, "initiatecustomerindemnity", StaffNameAndRole);
            return response;
        }


        /// <summary>
        ///  Initiate account indemnitylimit.
        ///  permission-initiateaccountindemnitylimit
        /// </summary>
        [Authorize(Policy = "initiateaccountindemnitylimit")]
        [HttpPost("InitiateAccountIndemnity/{username}/{AccountNumber}")]
        public async Task<GenericResponse> AccountIndemnityLimitAcceptance(string username, string AccountNumber)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            var response = await _mobileUserService.InitiateAccountIndemnityLimitAcceptance(username, "initiateaccountindemnitylimit", StaffNameAndRole,AccountNumber);
            return response;
        }


        /// <summary>
        /// upload customer indemnity form from back office.
        /// Pass the form with this name-indemnityform along with the request body.
        /// Note that this is form-data
        /// for IndemnityType use either accountindemnity or customerindemnity
        /// permission- indemnityformentryofficer
        /// </summary>
        [Authorize(Policy = "indemnityformentryofficer")]
        [HttpPost("IndemnityFormUploadForCustomer")]
        public async Task<GenericResponse2> IndemnityFormUploadForCustomer([FromHeader] string ClientKey, [FromForm] BackofficeIndemnityForm customerDocuments, [FromForm] IFormFile indemnityform)
        {
            
            var resp = await _authServ.BackOfficeIndemnityFormUploadForCustomer(ClientKey, customerDocuments, indemnityform);
            Console.WriteLine("UploadIndemnityForm ...." + resp.ToString());
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var StaffNameAndRole = JwtDecoder.DecodeJwtToken(token);
            if (resp.Success&&customerDocuments.IndemnityType.Equals("accountindemnity",StringComparison.CurrentCultureIgnoreCase)) {
            var response = await _mobileUserService.InitiateAccountIndemnityLimitAcceptance(customerDocuments.Username, "initiateaccountindemnitylimit", StaffNameAndRole,customerDocuments.AccountNumber);
            _genServ.LogRequestResponse("UploadIndemnityForm ...", null, JsonConvert.SerializeObject(resp));
            }else if(resp.Success&&customerDocuments.IndemnityType.Equals("customerindemnity", StringComparison.CurrentCultureIgnoreCase))
            {
                var response = await _mobileUserService.InitiateCustomerIndmnityLimitAcceptance(customerDocuments.Username, "initiatecustomerindemnity", StaffNameAndRole);
            }
            return resp;
        }


        /// <summary>
        /// Set limit as approved for customer . final leg . 
        /// role-Authorizer
        /// not in use anymore
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Authorizer")]
        [HttpGet("SetPendingCustomerAccountLimitUpdateAsApproved/{username}")]
        public async Task<GenericResponse2> SetPendingCustomerAccountLimitUpdateAsTreated(string username)
        {
            var response = await _mobileUserService.SetPendingCustomerAccountLimitUpdateAsTreated(username);
            return response;
        }
        
        /// <summary>
        /// Get Pending kyc list 
        /// </summary>
       // [Authorize(Roles = "admin,Authorizer,Initiator,viewer")]
        [HttpGet("GetPendingKyc")]
        public async Task<GenericResponse2> GetPendingKyc()
        {
            var response = await _mobileUserService.GetPendingKyc();
            return response;
        }


        /// <summary>
        /// Set Pending kyc as treated 
        /// role-Authorizer
        /// not in use anymore
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Authorizer")]
        [HttpPost("SetPendingKycAsTreated/{username}")]
        public async Task<GenericResponse2> SetPendingKycAsTreated(string username)
        {
            var response = await _mobileUserService.SetPendingKycAsTreated(username);
            return response;
        }
        

        /// <summary>
        /// Get user details by username
        /// </summary>
        [HttpGet("FetchUser/{UserName}")]
        public async Task<GenericResponse2> FetchUser(string UserName)
        {
            string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
            Console.WriteLine("host " + host);
            var response = await _mobileUserService.FetchUser(UserName,host);
            return response;
        }

        /// <summary>
        /// Update PhoneNumber and Email from Core banking
        /// </summary>
        [HttpGet("UpdatePhoneNumberAndEmail/{UserName}")]
        public async Task<GenericResponse2> UpdatePhoneNumberAndEmail(string UserName, [FromQuery] string PhoneNumber, [FromQuery] string Email)
        {
            //string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.PathBase;
           // Console.WriteLine("host " + host);
            var response = await _mobileUserService.UpdatePhoneNumberAndEmail(UserName,PhoneNumber,Email);
            return response;
        }

    }
}







































































































































































