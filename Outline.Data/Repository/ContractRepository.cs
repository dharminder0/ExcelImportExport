using Core.Common.Configuration;
using Core.Common.Data;
using Outline.Entity;
using Outline.Entity.DataModels;
using System.Diagnostics.Contracts;
using WebApplicationOutline.Repository;
using Contract = Outline.Entity.DataModels.Contract;

namespace WebApplicationOutline.Repository {
    public interface IContractRepository {
        int SaveContract(Contract contract);
        IEnumerable<ContractResponseModel> GetContractorRecordList();

        IEnumerable<Contract> GetContractors();
    }


    public class ContractRepository : DataRepository<Contract>, IContractRepository {
        private const string SaveContractSql = @"
IF EXISTS (SELECT id FROM Contract WHERE contractName = @ContractName)
BEGIN
    SELECT id FROM Contract WHERE contractName = @ContractName
END
ELSE
BEGIN
    INSERT INTO Contract (
        ContractName,
        Client,
        SingleMaster,
        JointVentureName,
        ShortName,
        ContactNumber,
        StartDT,
        EndDT,
        ContractManager,
        TimesheetVersionType
    )
    VALUES (
        @ContractName,
        @Client,
        @SingleMaster,
        @JointVentureName,
        @ShortName,
        @ContactNumber,
        @StartDT,
        @EndDT,
        @ContractManager,
        @TimesheetVersionType
    );
    SELECT SCOPE_IDENTITY();
END";

        private const string GetContractorRecordListSql = @"
SELECT  
    C.Id,
    ContractName,
    Client,
    SingleMaster,
    JointVentureName,
    C.ShortName,
    ContactNumber,
    StartDT,
    EndDT,
    ContractManager,
    TimesheetVersionType,
    ContractId,
    CommonLabourCategory,
    DisplayName
FROM Contract C
JOIN ContractLabour L ON C.Id = L.ContractId";

        private const string GetContractorsSql = @"
SELECT *
FROM Contract";

        public int SaveContract(Contract contract) {
            return ExecuteScalar<int>(SaveContractSql, contract);
        }

        public IEnumerable<ContractResponseModel> GetContractorRecordList() {
            return Query<ContractResponseModel>(GetContractorRecordListSql);
        }

        public IEnumerable<Contract> GetContractors() {
            return Query<Contract>(GetContractorsSql);
        }
    }
}
