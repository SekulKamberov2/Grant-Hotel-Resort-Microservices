IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DutyManagementDB') 
BEGIN 
	CREATE DATABASE DutyManagementDB; 
END 
GO

CREATE TABLE Duties (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title                 NVARCHAR(255) NOT NULL,
    Description           NVARCHAR(1000),
    
    AssignedToUserId      NVARCHAR(100) NOT NULL,  -- From Identity Server (GUID or sub)
    AssignedByUserId      NVARCHAR(100),           -- Optional: who assigned it

    RoleRequired          NVARCHAR(100),           -- Based on JWT "role" claim (e.g., 'bartender', 'cleaner')
    Facility              NVARCHAR(100),           -- E.g., 'Fitness Center', 'Bar'

    Status                NVARCHAR(50) DEFAULT 'Pending',  -- Pending, InProgress, Completed, Missed
    Priority              INT DEFAULT 2, -- 1 = High, 2 = Normal, 3 = Low

    DueDate               DATETIME,
    CompletionDate        DATETIME,

    CreatedAt             DATETIME DEFAULT GETDATE(),
    UpdatedAt             DATETIME DEFAULT GETDATE()
);
 GO

endpoints: reset-password, admin/reset-password, create-role, assign-role, update-role/id done