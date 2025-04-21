using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Retailbanking.BL.ActionFilter;
using Retailbanking.BL.IServices;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;

namespace PrimeAppAdmin.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly IGeneric _genServ;
        private readonly IAuthentication _authServ;
        private readonly FolderPaths _folderPaths;
        private readonly IStaffUserService _staffUserService;
        private readonly IMobileUserService _mobileUserService;
        private readonly IOfficeTransactionLoader _officeTransactionLoader;

        public TransactionController(IOfficeTransactionLoader officeTransactionLoader,IOptions<FolderPaths> folderPaths, IMobileUserService mobileUserService, IGeneric genServ, IAuthentication authServ, IStaffUserService staffUserService)
        {
            _genServ = genServ;
            _authServ = authServ;
            _folderPaths = folderPaths.Value;
            _staffUserService = staffUserService;
            _mobileUserService = mobileUserService;
            _officeTransactionLoader = officeTransactionLoader;
        }

        /// <summary>
        /// Get Mobile trans history.
        /// </summary>
        [HttpGet("FetchTransactions/{page}/{size}")]
        public async Task<GenericResponse> GetAUserMobileTrasanctionHistory([FromQuery] string transtype,int page, int size)
        {
            var response = await _officeTransactionLoader.FetchTransactions(transtype,page,size);
            return response;
        }

        /// <summary>
        /// Get Mobile trans history.
        /// date pattern- mm-dd-yyyy
        /// </summary>
        [HttpGet("FetchTransactionsBydate/{StartDate}/{EndDate}/{page}/{size}")]
        public async Task<GenericResponse> FetchTransactionsBydate([FromQuery] string transtype,string StartDate,string EndDate,int page, int size)
        {
            var response = await _officeTransactionLoader.FetchTransactionByDate(transtype,StartDate,EndDate,page,size);
            return response;
        }

        /// <summary>
        /// Search Mobile trans history.
        /// by source account
        /// </summary>
        [HttpGet("SearchTransactionsBySourceAccount/{SourceAccount}")]
        public async Task<GenericResponse> SearchTransactionsBySourceAccount(string SourceAccount)
        {
            var response = await _officeTransactionLoader.SearchTransactionsBySourceAccount(SourceAccount);
            return response;
        }

        /// <summary>
        /// Search Transaction by reference
        /// </summary>
        [HttpGet("SearchTransactionsByReference/{Reference}/{type}")]
        public async Task<GenericResponse2> SearchTransactionsByReference(string Reference,string type,[FromQuery] string UserName,[FromQuery] string SourceAccount)
        {
            var response = await _officeTransactionLoader.SearchTransactionsByReference(Reference,type,UserName,SourceAccount);
            return response;
        }

        [AllowAnonymous]
        [HttpGet("TestMe")]
        [ServiceFilter(typeof(AuthorizerActionFilter))] // Apply the filter here
        public async Task<GenericResponse> TestMe()
        {
           return _officeTransactionLoader.SendCustomDataToFilter();
          //  return new GenericResponse() { Response=EnumResponse.Successful,Success=true};
        }

    }
}
