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
    public class TransactionReportController : ControllerBase
    {
        
        private readonly ITransactionReportService _transactionReportService;

        public TransactionReportController(ITransactionReportService transactionReportService)
        {
            _transactionReportService = transactionReportService;
        }

        /// <summary>
        /// Report Transactions with Issues
        /// </summary>
        [HttpGet("ReportTransactions")]
        public async Task<GenericResponse> getTransactionWithIssues([FromQuery] int page , [FromQuery] int size)
        {
          return await _transactionReportService.getTransactionWithIssues(page, size);
        }

        /// <summary>
        /// Search Transactions with date.
        /// start and enddate format= dd-mm-yyyy
        /// </summary>
        [HttpGet("SearchTransactions")]
        public async Task<GenericResponse> SearchTransactionWithDate([FromQuery] string startdate, [FromQuery] string enddate)
        {
            return await _transactionReportService.SearchTransactionWithIssues(startdate,enddate);
        }

        /// <summary>
        /// Search Transactions with ref
        /// </summary>
        [HttpGet("SearchTransactionsByReference/{transactionref}")]
        public async Task<GenericResponse> SearchTransactionWithReference(string transactionref)
        {
            return await _transactionReportService.SearchTransactionWithReference(transactionref);
        }

        /// <summary>
        /// Mark transaction as fixed
        /// </summary>
        [HttpPost("ResolvedTransaction/{username}/{status}/{transactionref}")]
        public async Task<GenericResponse> ResolvedTransaction(string username,bool status,string transactionref)
        {
            return await _transactionReportService.ReportTransactionAsFixed(username,status,transactionref);
        }


        /// <summary>
        /// total transaction by transfer
        /// </summary>
        [HttpGet("TotalTransactionByTransfer")]
        public async Task<GenericResponse> TotalTransactionByTransfer()
        {
            return await _transactionReportService.TotalTransactionByTransfer();
        }

        /// <summary>
        /// total transaction by transfer and Bill
        /// </summary>
        [HttpGet("TotalTransactionByTransferAndBill")]
        public async Task<GenericResponse> TotalTransactionByTransferAndBill()
        {
            return await _transactionReportService.TotalTransactionByTransferAndBill();
        }

        /// <summary>
        /// total transaction by Bill
        /// </summary>
        [HttpGet("TotalTransactionByBill")]
        public async Task<GenericResponse> TotalTransactionByBill()
        {
            return await _transactionReportService.TotalTransactionByBill();
        }

        /// <summary>
        /// total customer
        /// </summary>
        [HttpGet("TotalCustomer")]
        public async Task<GenericResponse> TotalCustomer()
        {
            return await _transactionReportService.TotalCustomer();
        }

        /// <summary>
        /// total active or inactive customer
        /// pass for active or inactive for customer status
        /// </summary>
        [HttpGet("TotalActiveOrInActiveCustomer/{status}")]
        public async Task<GenericResponse> TotalActiveOrInActiveCustomer(string status)
        {
            return await _transactionReportService.TotalActiveOrInActiveCustomer(status);
        }

        /// <summary>
        /// total active and inactive customer
        /// </summary>
        [HttpGet("TotalActiveAndInActiveCustomer")]
        public async Task<GenericResponse> TotalActiveAndInActiveCustomer()
        {
            return await _transactionReportService.TotalActiveAndInActiveCustomer();
        }

    }
        
}










