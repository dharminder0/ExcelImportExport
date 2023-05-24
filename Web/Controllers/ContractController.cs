using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Outline.Entity;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationOutline.Repository;
using WebApplicationOutline.Service;
//using WebApplicationOutline.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace WebApplicationOutline.Controllers {
    public class ContractController : Controller {

        private readonly IContractService _contractService;

        public ContractController(IContractService contractService) {
            _contractService = contractService;
        }

        public IActionResult Index() {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file) {
            if (file == null || file.Length == 0) {
                ModelState.AddModelError("File", "Please select a file.");
                return BadRequest(ModelState);
            }

          
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) {
                ModelState.AddModelError("File", "Please upload an Excel file (.xlsx).");
                return BadRequest(ModelState);
            }

            var response = await _contractService.ImportData(file);
            if (response) {
                return RedirectToAction("ListView");
               // return Ok("File uploaded and data saved to the database successfully.");
            }
            else
                return Ok("Error in File upload.");
        }



        
        public async Task<IActionResult> ListView() {

            var recordList = _contractService.GetRecordList();
            return View(recordList);
        }

        public ActionResult ExportCSV() {
            var excelBytes = _contractService.ExportToExcel();
            return File(new MemoryStream(excelBytes), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Contract Import Sample.xlsx");
        }
    }
}
