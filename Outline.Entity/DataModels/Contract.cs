using Core.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outline.Entity.DataModels {
    [Alias(Name = "Contract")]
    public class Contract {
        [Key(AutoNumber = true)]
        public int Id { get; set; }

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
}
