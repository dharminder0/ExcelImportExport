using Core.Common.Data;


namespace Outline.Entity.DataModels {
    [Alias(Name = "ContractLabour")]
    public  class ContractLabour {
        [Key(AutoNumber = true)]
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string CommonLabourCategory { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public string EEO { get; set; }
    }
}
