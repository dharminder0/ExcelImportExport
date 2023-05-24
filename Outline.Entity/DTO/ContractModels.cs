namespace Outline.Entity {
    public class ContractBasicModel {
        public string ContractName { get; set; }
        public string Client { get; set; }
        public string SingleMaster { get; set; }
        public string JointVentureName { get; set; }
        public string ShortName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime StartDT { get; set; }
        public DateTime EndDT { get; set; }
        public string ContractManager { get; set; }
        public string TimesheetVersionType { get; set; }
    }
  


    public class LabourCategoryModel{
        public string ContractName { get; set; }

        public string CommonLabourCategory { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string EEO { get; set; }
    }

    public class ContractResponseModel : ContractBasicModel {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string CommonLabourCategory { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string EEO { get; set; }


    }

    public class ContractRequestModel: ContractBasicModel { 
    
    public List<LabourCategoryModel> LabourCategoryList { get; set; }

    }

}
