using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailbanking.BL.IServices;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;

namespace PrimeAppAdmin.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PrimeLoanController : ControllerBase
    {

        private readonly IGeneric _genServ;
        private readonly IMobileInvestment _mobileInvestment;

        public PrimeLoanController(IGeneric genServ, IMobileInvestment mobileInvestment)
        {
            _genServ = genServ;
            _mobileInvestment = mobileInvestment;
        }



        /// <summary>
        /// Get Retail loan
        /// </summary>
        [HttpGet("GetRetailLoan/{UserName}")]
        public async Task<GenericResponse> GetRetailLoan(string UserName)
        {
            return await _mobileInvestment.GetRetailLoan(UserName);
        }


        /// <summary>
        /// Get public sector loan
        /// </summary>
        [HttpGet("GetPublicSectorLoan/{UserName}")]
        public async Task<GenericResponse> GetPublicSectorLoan(string UserName)
        {
            return await _mobileInvestment.GetPublicSectorLoan(UserName);
        }

    }
}
