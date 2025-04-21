using Microsoft.AspNetCore.Mvc;
using Retailbanking.BL.IServices;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace PrimeAppAdmin.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PrimeInvestmentController : ControllerBase
    {

        private readonly IGeneric _genServ;
        private readonly IMobileInvestment _mobileInvestment;

        public PrimeInvestmentController(IGeneric genServ, IMobileInvestment mobileInvestment)
        {
            _genServ = genServ;
            _mobileInvestment = mobileInvestment;
        }

        /// <summary>
        /// Get mobile Active FixedDeposit or Halal Investment
        /// </summary>
        [HttpGet("GetActiveFixedDepositOrHalalaInvestment/{UserName}")]
        public async Task<GenericResponse> GetActiveFixedDepositInvestment(string UserName)
        {
            return await _mobileInvestment.GetActiveFixedDepositOrHalalaInvestment(UserName);
        }


        /// <summary>
        /// FixedDeposit or Halal Investment history.
        /// This will fetch all with status: Active, underprocessing,and liquidated
        /// </summary>
        [HttpGet("GetAllFixedDepositOrHalaInvestmentHistory/{UserName}/{InvestmentType}")]
        public async Task<GenericResponse> GetAllFixedDepositOrHalalaInvestmentHistory(string UserName,string InvestmentType)
        {
            return await _mobileInvestment.GetAllFixedDepositHistory(UserName, InvestmentType);
        }


        /// <summary>
        /// Get Prime Mutualfund
        /// </summary>
        [HttpGet("GetMutualFundInvesment/{UserName}")]
        public async Task<GenericResponse> GetMutualFundInvesment(string UserName)
        {
            return await _mobileInvestment.GetMutualFundInvestment(UserName);
        }

    }
}






































































































































































































































