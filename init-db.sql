IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'IdentityGHRDB')
BEGIN
    CREATE DATABASE IdentityGHRDB;
END
GO

USE IdentityGHRDB;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(255) NOT NULL UNIQUE,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        PhoneNumber NVARCHAR(256),
        DateCreated DATETIME DEFAULT GETDATE()
    );

    CREATE TABLE Roles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL UNIQUE,
        Description NVARCHAR(500),
        DateCreated DATETIME DEFAULT GETDATE()
    );

    CREATE TABLE UserRoles (
        UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
        CONSTRAINT UC_UserRole UNIQUE (UserId, RoleId)
    ); 

    INSERT INTO Users (Username, Email, PasswordHash, PhoneNumber)
    VALUES (
        'Sekul',
        'sekul7@gmail.com',
        'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=',
        '+1234567890'
    );

    COMMIT TRAN


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