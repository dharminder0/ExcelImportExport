Create Table Contract (
        id int identity(1,1) Primary key 
       , ContractName nvarchar(100)
        ,Client nvarchar(100)
        ,SingleMaster nvarchar(100)
        ,JointVentureName nvarchar(100)
        ,ShortName nvarchar(100)
        ,ContactNumber nvarchar(100)
        ,StartDT DateTime
        ,EndDT DateTime
        ,ContractManager nvarchar(100)
        ,TimesheetVersionType nvarchar(100)
)