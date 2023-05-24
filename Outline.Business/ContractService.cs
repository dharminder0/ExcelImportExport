using OfficeOpenXml;
using Outline.Entity;
using Outline.Entity.DataModels;
using WebApplicationOutline.Repository;
using Contract = Outline.Entity.DataModels.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Amqp.Framing;

namespace WebApplicationOutline.Service {
    public interface IContractService {
        bool SaveContract(List<ContractRequestModel> contractList);
        IEnumerable<ContractResponseModel> GetRecordList();
        byte[] ExportToExcel();
        Task<bool> ImportData(IFormFile file);

    }

    public class ContractService : IContractService {
        private readonly ILabourCategoryRepository _labourCategoryRepository;
        private readonly IContractRepository _contractRepository;
        public ContractService(ILabourCategoryRepository labourCategoryRepository, IContractRepository contractRepository) {
            _labourCategoryRepository = labourCategoryRepository;
            _contractRepository = contractRepository;
        
        }

        public bool SaveContract(List<ContractRequestModel> contractList) {
            if (contractList == null || !contractList.Any()) {
                return false;
            }

            foreach (var requestModel in contractList) {
                if (requestModel == null) {
                    continue;
                }

                Contract contract = MapToContract(requestModel);

                int contractId = _contractRepository.SaveContract(contract);
                if (contractId > 0 && requestModel.LabourCategoryList != null && requestModel.LabourCategoryList.Any()) {
                    foreach (var item in requestModel.LabourCategoryList) {
                        var contractLabour = MapToContractLabour(contractId, item);
                        _labourCategoryRepository.AddContractLabourCategory(contractLabour);
                    }

                    return true;
                }
            }

            return false;
        }
        public IEnumerable<ContractResponseModel> GetRecordList() {
            return _contractRepository.GetContractorRecordList();
        }
        public async Task<bool> ImportData(IFormFile file) {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            var conractList = new List<ContractRequestModel>();
            using (var package = new ExcelPackage(new FileInfo(filePath))) {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) {
                    if (TryParseContractRequestModel(worksheet, row, out ContractRequestModel contractor)) {
                        conractList.Add(contractor);
                    }
                }

                var worksheetLabour = package.Workbook.Worksheets[1];
                var rowCountLabour = worksheetLabour.Dimension.Rows;
                for (int row = 2; row <= rowCountLabour; row++) {
                    if (TryParseLabourCategoryModel(worksheetLabour, row, out LabourCategoryModel labourCategoryModel)) {
                        var employeeExist = conractList.FirstOrDefault(v => v.ContractName.Equals(labourCategoryModel.ContractName));

                        if (employeeExist != null) {
                            employeeExist.LabourCategoryList.Add(labourCategoryModel);
                        }
                    }
                }

                SaveContract(conractList);
            }

            return true;
        }

        public byte[] ExportToExcel() {
            var contractList = _contractRepository.GetContractors();
            var contractlabourList = _labourCategoryRepository.Contractlabour();

            using (ExcelPackage excelPackage = new ExcelPackage()) {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Contract Basic Info");

                SetContractBasicInfoHeader(worksheet);
                FillContractBasicInfoData(worksheet, contractList);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                worksheet = excelPackage.Workbook.Worksheets.Add("Labour Category");

                SetLabourCategoryHeader(worksheet);
                FillLabourCategoryData(worksheet, contractList, contractlabourList);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                byte[] excelBytes = excelPackage.GetAsByteArray();
                return excelBytes;
            }
        }

        private Contract MapToContract(ContractRequestModel requestModel) {
            return new Contract {
                ContractName = requestModel.ContractName,
                Client = requestModel.Client,
                SingleMaster = requestModel.SingleMaster,
                JointVentureName = requestModel.JointVentureName,
                ShortName = requestModel.ShortName,
                ContactNumber = requestModel.ContactNumber,
                StartDT = requestModel.StartDT,
                EndDT = requestModel.EndDT,
                ContractManager = requestModel.ContractManager,
                TimesheetVersionType = requestModel.TimesheetVersionType
            };
        }

        private ContractLabour MapToContractLabour(int contractId, LabourCategoryModel item) {
            return new ContractLabour {
                ContractId = contractId,
                CommonLabourCategory = item.CommonLabourCategory,
                DisplayName = item.DisplayName,
                ShortName = item.CommonLabourCategory,
                EEO = item.EEO
            };
        }


     

        private bool TryParseContractRequestModel(ExcelWorksheet worksheet, int row, out ContractRequestModel contractor) {
            contractor = new ContractRequestModel();

            DateTime.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out DateTime startDT);
            DateTime.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out DateTime endDT);

            contractor.Client = worksheet.Cells[row, 1].Value?.ToString();
            contractor.SingleMaster = worksheet.Cells[row, 2].Value?.ToString();
            contractor.JointVentureName = worksheet.Cells[row, 3].Value?.ToString();
            contractor.ContractName = worksheet.Cells[row, 4].Value?.ToString();
            contractor.ShortName = worksheet.Cells[row, 5].Value?.ToString();
            contractor.ContactNumber = worksheet.Cells[row, 6].Value?.ToString();
            contractor.StartDT = startDT;
            contractor.EndDT = endDT;
            contractor.ContractManager = worksheet.Cells[row, 10].Value?.ToString();
            contractor.TimesheetVersionType = worksheet.Cells[row, 11].Value?.ToString();
            contractor.LabourCategoryList = new List<LabourCategoryModel>();

            return !string.IsNullOrEmpty(contractor.ContractName);
        }

        private bool TryParseLabourCategoryModel(ExcelWorksheet worksheet, int row, out LabourCategoryModel labourCategoryModel) {
            labourCategoryModel = new LabourCategoryModel();

            labourCategoryModel.ContractName = worksheet.Cells[row, 1].Value?.ToString();
            labourCategoryModel.CommonLabourCategory = worksheet.Cells[row, 2].Value?.ToString();
            labourCategoryModel.DisplayName = worksheet.Cells[row, 3].Value?.ToString();
            labourCategoryModel.ShortName = worksheet.Cells[row, 4].Value?.ToString();
            labourCategoryModel.EEO = worksheet.Cells[row, 5].Value?.ToString();

            return !string.IsNullOrEmpty(labourCategoryModel.ContractName);
        }


       

        private void SetContractBasicInfoHeader(ExcelWorksheet worksheet) {
            worksheet.Cells[1, 1].Value = "Client";
            worksheet.Cells[1, 2].Value = "Single/Master";
            worksheet.Cells[1, 3].Value = "Joint Venture";
            worksheet.Cells[1, 4].Value = "Name";
            worksheet.Cells[1, 5].Value = "shortName";
            worksheet.Cells[1, 6].Value = "Contact Number";
            worksheet.Cells[1, 8].Value = "StartDT";
            worksheet.Cells[1, 9].Value = "EndDT";
            worksheet.Cells[1, 10].Value = "Contract Manager";
            worksheet.Cells[1, 11].Value = "Timesheet Version Type";
        }

        private void FillContractBasicInfoData(ExcelWorksheet worksheet, IEnumerable<Contract> contractList) {
            int row = 2;
            foreach (var contract in contractList) {
                worksheet.Cells[row, 1].Value = contract.Client;
                worksheet.Cells[row, 2].Value = contract.SingleMaster;
                worksheet.Cells[row, 3].Value = contract.JointVentureName;
                worksheet.Cells[row, 4].Value = contract.ContractName;
                worksheet.Cells[row, 5].Value = contract.ShortName;
                worksheet.Cells[row, 6].Value = contract.ContactNumber;
                worksheet.Cells[row, 8].Value = contract.StartDT;
                worksheet.Cells[row, 9].Value = contract.EndDT;
                worksheet.Cells[row, 10].Value = contract.ContractManager;
                worksheet.Cells[row, 11].Value = contract.TimesheetVersionType;

                row++;
            }
        }

        private void SetLabourCategoryHeader(ExcelWorksheet worksheet) {
            worksheet.Cells[1, 1].Value = "Contract Name";
            worksheet.Cells[1, 2].Value = "Common Labour Category";
            worksheet.Cells[1, 3].Value = "Display name";
            worksheet.Cells[1, 4].Value = "Short Name";
            worksheet.Cells[1, 5].Value = "EEO";
        }

        private void FillLabourCategoryData(ExcelWorksheet worksheet, IEnumerable<Contract> contractList, IEnumerable<ContractLabour> contractlabourList) {
            int row = 2;
            foreach (var contract in contractlabourList) {
                var contractName = contractList.FirstOrDefault(v => v.Id.Equals(contract.ContractId))?.ContractName;
                worksheet.Cells[row, 1].Value = contractName;
                worksheet.Cells[row, 2].Value = contract.CommonLabourCategory;
                worksheet.Cells[row, 3].Value = contract.DisplayName;
                worksheet.Cells[row, 4].Value = contract.ShortName;
                worksheet.Cells[row, 5].Value = contract.EEO;
                row++;
            }
        }

  


    }
}
