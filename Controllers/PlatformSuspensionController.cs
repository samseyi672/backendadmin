using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailbanking.BL.IServices;
using Retailbanking.Common.CustomObj;
using System.Threading.Tasks;
using System.Transactions;

namespace PrimeAppAdmin.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PlatformSuspensionController : ControllerBase
    {
        private readonly IPlatformSuspenderService _platformSuspenderService;

        public PlatformSuspensionController(IPlatformSuspenderService platformSuspenderService)
        {
            _platformSuspenderService = platformSuspenderService;
        }


        /// <summary>
        /// Get suspension status of the platform
        /// </summary>
        [HttpGet("PlatformSuspensionStatus")]
        public async Task<GenericResponse2> getPlatformSuspensionStatus()
        {
            return await _platformSuspenderService.getPlatformSuspensionStatus();
        }

        /// <summary>
        /// Set suspension status of the platform for login
        /// </summary>
       // [Authorize(Policy = "turnonloginoroff")]
        [HttpPost("SetPlatformLoginStatus/{login}")]
        public async Task<GenericResponse2> SetPlatformLogin(bool login)
        {         
            return await _platformSuspenderService.SetPlatformSuspensionForLogin(login,false,false);
        }

        /// <summary>
        /// Set suspension status of the platform for transaction
        /// </summary>
        [Authorize(Policy = "turnontransactionoroff")]
        [HttpPost("SetPlatformTransactionStatus/{transaction}")]
        public async Task<GenericResponse2> SetPlatformTransaction(bool transaction)
        {
            return await _platformSuspenderService.SetPlatformSuspensionForTransactionStatus(false,transaction,false);
        }

        /// <summary>
        /// Set suspension status of the platform for bills
        /// </summary>
        [Authorize(Policy = "turnonbilloroff")]
        [HttpPost("SetPlatformBills/{bills}")]
        public async Task<GenericResponse2> SetPlatformBills(bool bills)
        {
            return await _platformSuspenderService.SetPlatformSuspensionForBills(false,false,bills);
        }

    }
}











































































































































































































