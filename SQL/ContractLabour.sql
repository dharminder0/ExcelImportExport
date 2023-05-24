CREATE TABLE [dbo].[ContractLabour] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [ContractId]           INT            NULL,
    [CommonLabourCategory] NVARCHAR (100) NULL,
    [DisplayName]          NVARCHAR (100) NULL,
    [ShortName]            NVARCHAR (100) NULL,
    [EEO]                  NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_ContractLabour_contract] FOREIGN KEY (ContractId) REFERENCES Contract(Id)
);

