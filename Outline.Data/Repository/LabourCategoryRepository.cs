using Core.Common.Data;
using Outline.Entity.DataModels;
using WebApplicationOutline.Repository;

namespace WebApplicationOutline.Repository {
    public interface ILabourCategoryRepository {
        bool AddContractLabourCategory(ContractLabour labourCategoryModel);
        IEnumerable<ContractLabour> Contractlabour();
    }


    public class LabourCategoryRepository : DataRepository<ContractLabour>, ILabourCategoryRepository {
        private const string AddContractLabourCategorySql = @"
IF NOT EXISTS (SELECT 1 FROM Contractlabour WHERE CommonLabourCategory = @CommonLabourCategory AND ContractId = @ContractId)
BEGIN
    INSERT INTO Contractlabour (
        ContractId,
        CommonLabourCategory,
        DisplayName,
        ShortName,
        EEO
    )
    VALUES (
        @ContractId,
        @CommonLabourCategory,
        @DisplayName,
        @ShortName,
        @EEO
    )
END
ELSE
BEGIN
    UPDATE Contractlabour
    SET DisplayName = @DisplayName,
        ShortName = @ShortName
    WHERE CommonLabourCategory = @CommonLabourCategory
        AND ContractId = @ContractId
END";

        private const string GetContractlabourSql = @"
SELECT *
FROM Contractlabour";

        public bool AddContractLabourCategory(ContractLabour labourCategoryModel) {
            return Execute(AddContractLabourCategorySql, labourCategoryModel) > 0;
        }

        public IEnumerable<ContractLabour> Contractlabour() {
            return Query<ContractLabour>(GetContractlabourSql);
        }
    }
}
