 ## SQL

 ```sql

-- DFM (Department Facility Management)
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'DFMDB'
)
BEGIN
    CREATE DATABASE DFMDB;
END

USE DFMDB;
GO

BEGIN TRANSACTION; 

	BEGIN TRY
		CREATE TABLE Facilities (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			Name NVARCHAR(100) NOT NULL,
			Description NVARCHAR(500),
			Location NVARCHAR(100),
			Department NVARCHAR(100), -- Housekeeping, Maintenance
			Status NVARCHAR(50),       -- Active, Under Maintenance 
			CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
			UpdatedAt DATETIME2 NULL
		);
	 
		CREATE TABLE FacilitySchedules (
			ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
			FacilityId INT NOT NULL,
			DayOfWeek INT NOT NULL, -- 0=Sunday, 1=Monday, ...
			OpenTime TIME NOT NULL,
			CloseTime TIME NOT NULL,
			IsMaintenance BIT NOT NULL DEFAULT 0,
			FOREIGN KEY (FacilityId) REFERENCES Facilities(Id)
		); 

		CREATE TABLE FacilityServices (
			ServiceId INT IDENTITY(1,1) PRIMARY KEY,
			FacilityId INT NOT NULL,
			Name NVARCHAR(100) NOT NULL,
			Description NVARCHAR(500),
			Price DECIMAL(10,2),
			DurationMinutes INT,
			FOREIGN KEY (FacilityId) REFERENCES Facilities(Id)
		);

		CREATE TABLE FacilityReservations (
			ReservationId INT IDENTITY(1,1) PRIMARY KEY,
			FacilityId INT NOT NULL,
			ReservedBy NVARCHAR(100) NOT NULL,
			ReservationDate DATE NOT NULL,
			StartTime TIME NOT NULL,
			EndTime TIME NOT NULL,
			Purpose NVARCHAR(200),
			CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
			FOREIGN KEY (FacilityId) REFERENCES Facilities(Id)
		); 
		
		CREATE TABLE FacilityIssues (
			IssueId INT IDENTITY(1,1) PRIMARY KEY,
			FacilityId INT NOT NULL,
			ReportedBy NVARCHAR(100) NOT NULL,
			Description NVARCHAR(1000) NOT NULL,
			Status NVARCHAR(50) NOT NULL DEFAULT 'Open', -- Open, In Progress, Resolved
			ReportedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
			AssignedTo NVARCHAR(100) NULL,
			ResolvedAt DATETIME2 NULL,
			FOREIGN KEY (FacilityId) REFERENCES Facilities(Id)
		);  

		COMMIT TRANSACTION;
		PRINT 'All tables created successfully.';

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		PRINT 'Error occurred. Transaction rolled back.';
		PRINT ERROR_MESSAGE();
	END CATCH; 

-- RoomManagementDB
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'RoomManagementDB'
)
BEGIN
    CREATE DATABASE RoomManagementDB;
END

USE RoomManagementDB;

BEGIN TRANSACTION;  

	BEGIN TRY
 
		CREATE TABLE RoomType (
			Id INT IDENTITY PRIMARY KEY,
			Name NVARCHAR(50) NOT NULL,
			Description NVARCHAR(255)
		);
 
		CREATE TABLE Room (
			Id INT IDENTITY PRIMARY KEY,
			RoomNumber NVARCHAR(10) NOT NULL,
			Floor INT NOT NULL,
			TypeId INT NOT NULL FOREIGN KEY REFERENCES RoomType(Id),
			Status NVARCHAR(20) NOT NULL,  -- Available, Occupied, Maintenance, Cleaning
			Description NVARCHAR(255)
		);
 
		CREATE TABLE RoomRate (
			Id INT IDENTITY PRIMARY KEY,
			RoomTypeId INT NOT NULL FOREIGN KEY REFERENCES RoomType(Id),
			PricePerNight DECIMAL(10,2) NOT NULL,
			ValidFrom DATE NOT NULL,
			ValidTo DATE
		);
  
 
		CREATE TABLE Reservation (
			Id INT IDENTITY PRIMARY KEY,
			GuestId INT NOT NULL, --it is UserId from IdentityServer
			RoomId INT NOT NULL FOREIGN KEY REFERENCES Room(Id),
			CheckInDate DATE NOT NULL,
			CheckOutDate DATE NOT NULL,
			Status NVARCHAR(20) NOT NULL,  -- Pending, Confirmed, Cancelled, CheckedIn, CheckedOut
			CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
		);
 
		CREATE TABLE Payment (
			Id INT IDENTITY PRIMARY KEY,
			ReservationId INT NOT NULL FOREIGN KEY REFERENCES Reservation(Id),
			Amount DECIMAL(10,2) NOT NULL,
			PaymentMethod NVARCHAR(20) NOT NULL,
			PaidAt DATETIME NOT NULL DEFAULT GETDATE(),
			TransactionId NVARCHAR(100)
		); 
 
		CREATE TABLE CheckIn (
			Id INT IDENTITY PRIMARY KEY,
			ReservationId INT NOT NULL FOREIGN KEY REFERENCES Reservation(Id),
			PerformedByEmployeeId INT NOT NULL,
			Timestamp DATETIME NOT NULL DEFAULT GETDATE()
		);
 
		CREATE TABLE CheckOut (
			Id INT IDENTITY PRIMARY KEY,
			ReservationId INT NOT NULL FOREIGN KEY REFERENCES Reservation(Id),
			PerformedByEmployeeId INT NOT NULL,
			Timestamp DATETIME NOT NULL DEFAULT GETDATE()
		);
  
		COMMIT TRANSACTION;
		PRINT 'All tables created successfully.';

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		PRINT 'Error occurred. Transaction rolled back.';
		PRINT ERROR_MESSAGE();
	END CATCH;

  
INSERT INTO RoomType (Name, Description)
VALUES 
('Standard', 'Standard room with basic amenities'),
('Deluxe', 'Spacious deluxe room with sea view'),
('Suite', 'Luxury suite with separate living area');

INSERT INTO Room (RoomNumber, Floor, TypeId, Status, Description)
VALUES 
('101', 1, 1, 'Available', 'Standard room'),
('102', 1, 2, 'Cleaning', 'Deluxe room sea-facing'),
('201', 2, 3, 'Occupied', 'Luxury suite');

INSERT INTO RoomRate (RoomTypeId, PricePerNight, ValidFrom, ValidTo)
VALUES 
(1, 100.00, '2025-01-01', NULL),
(2, 150.00, '2025-01-01', NULL),
(3, 250.00, '2025-01-01', NULL);

INSERT INTO Reservation (GuestId, RoomId, CheckInDate, CheckOutDate, Status)
VALUES 
(1, 1, '2025-06-10', '2025-06-15', 'Confirmed'),
(1, 2, '2025-07-01', '2025-07-05', 'Pending');

INSERT INTO Payment (ReservationId, Amount, PaymentMethod, TransactionId)
VALUES 
(1, 500.00, 'CreditCard', 'TXN123456789'),
(2, 300.00, 'PayPal', 'TXN987654321');

INSERT INTO CheckIn (ReservationId, PerformedByEmployeeId)
VALUES 
(1, 1);

INSERT INTO CheckOut (ReservationId, PerformedByEmployeeId)
VALUES 
(1, 1);


-- RatingGHRDB

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RatingGHRDB')
BEGIN
    CREATE DATABASE RatingGHRDB;
END
GO

USE RatingGHRDB;

BEGIN TRANSACTION;

BEGIN TRY 
 
    CREATE TABLE Departments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL UNIQUE
    );
 
    INSERT INTO Departments (Name)
    VALUES ('Bar'), ('Casino'), ('Disco Club'), ('Duty'),
           ('Fitness & Spa'), ('Help Desk'), ('HR Platform'), ('Restaurant');
 
    CREATE TABLE Awards (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UsersId INT NOT NULL,
        DepartmentId INT NOT NULL,
        Title NVARCHAR(100),
        Period VARCHAR(20) CHECK (Period IN ('Weekly', 'Monthly', 'Yearly')),
        Date DATE,
        CONSTRAINT FK_Awards_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
    );
 
    CREATE TABLE Ratings (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        ServiceId INT NOT NULL, --  Fitness Trainer, HR Agent, Help Desk
        DepartmentId INT NOT NULL,
        Stars INT NOT NULL CHECK (Stars BETWEEN 1 AND 10),
        Comment NVARCHAR(1000) NULL,
        RatingDate DATETIME NOT NULL DEFAULT GETDATE(),

        IsApproved BIT NOT NULL DEFAULT 0,            -- Approval status for moderation
        IsDeleted BIT NOT NULL DEFAULT 0,             -- Soft delete flag
        ApprovedAt DATETIME NULL,                     -- When it was approved
        ApprovedBy INT NULL,                          -- Who approved (AdminId or StaffId)
        IsFlagged BIT NOT NULL DEFAULT 0,             -- Flag for moderation
        FlagReason NVARCHAR(500) NULL,                -- Reason for flagging

        CONSTRAINT FK_Ratings_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
    );

    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

-- HelpDeskGHRDB

IF DB_ID('HelpDeskGHRDB') IS NULL
BEGIN
    PRINT 'Creating HelpDeskGHRDB...';
    EXEC('CREATE DATABASE HelpDeskGHRDB');
END
ELSE
BEGIN
    PRINT 'HelpDeskGHRDB already exists.';
END

BEGIN TRANSACTION; 

USE HelpDeskGHRDB;

CREATE TABLE Departments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) UNIQUE NOT NULL -- Rooms, Spa, Casino, Pool, Bar
);
INSERT INTO Departments (DepartmentName) VALUES 
('Rooms'), ('Spa'), ('Casino'), ('Pool'), ('Bar'), ('Hotel'), ('Disco'), ('Beach'), ('Shop');

CREATE TABLE Locations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LocationName NVARCHAR(100) NOT NULL,
    DepartmentId INT NOT NULL,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);
INSERT INTO Locations (LocationName, DepartmentId) VALUES 
('Hotel', 1), ('Spa center', 2), ('Casino', 3), ('Big Pool', 4);   
 
CREATE TABLE Staff (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    DepartmentId INT,
    Role NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);

INSERT INTO Staff (FullName, Email, Phone, DepartmentId, Role, IsActive) VALUES 
('Sekul Kamberov', 'sekul1@gmail.com', '++123456789', 1, 1, 1); 

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
INSERT INTO Categories (CategoryName) VALUES 
('Hotel Rooms'), ('Spa center'), ('Casino'), ('Big Pool');

CREATE TABLE Priorities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PriorityName NVARCHAR(50) NOT NULL
);
INSERT INTO Priorities (PriorityName) VALUES 
('Critical'), ('High'), ('Normal'), ('Low');

CREATE TABLE Statuses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);
INSERT INTO Statuses (StatusName) VALUES 
('Open'), ('In Progress'), ('On Hold'), ('Resolved'), ('Closed'), ('Escalated'); 

CREATE TABLE TicketTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO TicketTypes (TypeName) VALUES
('Room Service'),
('Housekeeping'),
('Maintenance'),
('Front Desk'),
('Billing'),
('Other');

CREATE TABLE Tickets (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000),
    UserId INT NOT NULL,
    StaffId INT NULL,
    DepartmentId INT NOT NULL,
    LocationId INT NULL,
    CategoryId INT NOT NULL,
    PriorityId INT NOT NULL,
    StatusId INT NOT NULL DEFAULT 1,
    TicketTypeId INT NOT NULL FOREIGN KEY REFERENCES TicketTypes(Id),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL, 
    FOREIGN KEY (StaffId) REFERENCES Staff(Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id),
    FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (PriorityId) REFERENCES Priorities(Id),
    FOREIGN KEY (StatusId) REFERENCES Statuses(Id),
    FOREIGN KEY (TicketTypeId) REFERENCES TicketTypes(Id)
);

CREATE TABLE TicketLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TicketId INT NOT NULL,
    Comment NVARCHAR(1000),
    CreatedBy INT NOT NULL, --UserId
    CreatedByRole NVARCHAR(10),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id)
);
 
CREATE TABLE TicketRatings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TicketId INT NOT NULL,
    RatedByUserId NVARCHAR(450) NOT NULL,
    Score INT NOT NULL CHECK (Score >= 1 AND Score <= 5),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE
);

CREATE INDEX IX_TicketRatings_TicketId ON TicketRatings(TicketId);
CREATE INDEX IX_Tickets_AssignedUserId ON Tickets(StaffId);
 
CREATE TABLE TicketComments
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL,
    Text NVARCHAR(1000) NOT NULL,
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TicketComments_Tickets FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
 );

COMMIT TRANSACTION;

--Seeding Tickets
 DECLARE @i INT = 0;

WHILE @i < 100
BEGIN
    INSERT INTO Tickets (
        Title,
        Description,
        UserId,
        StaffId,
        DepartmentId,
        LocationId,
        CategoryId,
        PriorityId,
        StatusId,
        TicketTypeId,
        CreatedAt
    )
    SELECT
        'Test Ticket ' + CAST(@i AS NVARCHAR),
        'Random Description for ticket #' + CAST(@i AS NVARCHAR),
        ABS(CHECKSUM(NEWID())) % 10 + 1,  -- Assuming User IDs 1-10 exist
        (SELECT TOP 1 Id FROM Staff ORDER BY NEWID()), -- Get valid random StaffId
        (SELECT TOP 1 Id FROM Departments ORDER BY NEWID()),
        (SELECT TOP 1 Id FROM Locations ORDER BY NEWID()),
        (SELECT TOP 1 Id FROM Categories ORDER BY NEWID()),
        (SELECT TOP 1 Id FROM Priorities ORDER BY NEWID()),
        (SELECT TOP 1 Id FROM Statuses ORDER BY NEWID()),
        (SELECT TOP 1 Id FROM TicketTypes ORDER BY NEWID()),
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE());

    SET @i = @i + 1;
END
 

-- LeaveManagementGHRDB

IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'LeaveManagementGHRDB'
)
BEGIN
    CREATE DATABASE [LeaveManagementGHRDB];
END

USE LeaveManagementGHRDB;

CREATE TABLE LeaveTypes (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,       
    Description NVARCHAR(255),
    MaxDaysPerYear INT NOT NULL
);
 
 INSERT INTO LeaveTypes (Name, Description, MaxDaysPerYear)
 VALUES 
('Annual', 'Annual Leave: Paid Time Off (PTO). Purpose: Vacation, rest, or personal time.', 25),
('Sick', 'Sick Leave Purpose: For illness, injury, or medical appointments. Paid: Often yes, but policies vary.', 25),
('Maternity', 'Purpose: For childbirth and postnatal recovery. Paid: Varies by country/employer. Duration: Usually ranges from 12 weeks to 6 months.', 182),


CREATE TABLE LeaveBalances (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    LeaveTypeId INT NOT NULL,
    RemainingDays DECIMAL(5,2) NOT NULL,
    LastUpdated DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id),
    CONSTRAINT UC_LeaveBalance UNIQUE(UserId, Id)
);

CREATE TABLE LeaveApplications (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,                  -- Leave applicant
	FullName NVARCHAR(1000),
	PhoneNumber NVARCHAR(10),
	Email NVARCHAR(30),
	Department INT NOT NULL, 
    LeaveTypeId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalDays DECIMAL(5,2) NOT NULL,
    Reason NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
    ApproverId INT,                        -- Approving manager or admin
    DecisionDate DATETIME,
    RequestedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes(Id) 
);

CREATE TABLE Holidays (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Date DATE NOT NULL,
    IsRecurring BIT DEFAULT 0
);

-- DutyManagementDB
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DutyManagementDB') 
BEGIN 
	CREATE DATABASE DutyManagementDB; 
END

USE DutyManagementDB; 

BEGIN TRY
	BEGIN TRANSACTION;

		CREATE TABLE Duties (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			Title NVARCHAR(255) NOT NULL,
			Description NVARCHAR(1000),
			AssignedToUserId INT NOT NULL,
			AssignedByUserId INT NOT NULL,
			RoleRequired NVARCHAR(100),
			Facility NVARCHAR(100),
			Status NVARCHAR(50) DEFAULT 'Pending',
			Priority INT DEFAULT 2,
			DueDate DATETIME,
			CompletionDate DATETIME,
			CreatedAt DATETIME DEFAULT GETDATE(),
			UpdatedAt DATETIME DEFAULT GETDATE()
		); 

		CREATE TABLE Shifts (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			Name NVARCHAR(50) NOT NULL
		);

		INSERT INTO Shifts (Name) VALUES ('Morning'), ('Afternoon'), ('Night');
	 

		CREATE TABLE PeriodType (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			Name NVARCHAR(50) NOT NULL
		);

		INSERT INTO PeriodType (Name) VALUES ('Week'), ('Month'), ('Day');
 

	 
		CREATE TABLE DutyAssignments (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			EmployeeId INT NOT NULL,
			PeriodTypeId INT NOT NULL,
			DutyId INT NOT NULL,
			ShiftId INT NOT NULL,
			AssignmentDate DATE NOT NULL,
			FOREIGN KEY (PeriodTypeId) REFERENCES PeriodType(Id), 
			FOREIGN KEY (DutyId) REFERENCES Duties(Id),
			FOREIGN KEY (ShiftId) REFERENCES Shifts(Id)
		); 

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION;
	PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH;

-- IdentityGHRDB
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'IdentityGHRDB') 
BEGIN 
	CREATE DATABASE IdentityGHRDB; 
END 
  
USE IdentityGHRDB; 
  
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

SET IDENTITY_INSERT Roles ON;
 
    INSERT INTO Roles (Id, Name, Description)
    VALUES (1, 'EMPLOYEE', 'Basic employee role'); 
    INSERT INTO Roles (Id, Name, Description)
    VALUES (2, 'MANAGER', 'Manager role');  
    INSERT INTO Roles (Id, Name, Description)
    VALUES (3, 'HR ADMIN', 'Human Resources Admin role');
    INSERT INTO Roles (Id, Name, Description) VALUES 
    (4, 'HOUSEKEEPER', 'Room Attendant'),
    (5, 'HOUSEKEEPER MANAGER', 'Room Attendant Manager'),
    (6, 'HOTEL GUEST', 'HOTEL GUEST is a client');
  
SET IDENTITY_INSERT Roles OFF;

-- Qwerty1!@% is hashed password
INSERT INTO Users (Username, Email, PasswordHash, PhoneNumber)
VALUES 
('Sekul1', 'sekul1@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456789'),
('Sekul2', 'sekul2@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456788'),
('Sekul3', 'sekul3@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456787'),
('Sekul4', 'sekul4@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456789'),
('Sekul5', 'sekul5@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456788'),
('Sekul6', 'sekul6@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456787'),
('Sekul7', 'sekul7@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456789'),
('Sekul8', 'sekul8@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456788'),
('Sekul9', 'sekul9@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456787'),
('Sekul10', 'sekul10@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456789'),
('Sekul11', 'sekul11@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456788'),
('Sekul12', 'sekul12@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456787')
GO
 
COMMIT TRANSACTION;

END TRY BEGIN CATCH ROLLBACK TRANSACTION;

DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
RAISERROR(@ErrorMessage, 16, 1);

END CATCH;
