-- ============================================
-- STEP 1: Create databases if they don't exist
-- (This step is auto-commit and cannot be rolled back)
-- ============================================
PRINT 'Creating databases if not exist...';

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DFMDB')
    CREATE DATABASE DFMDB;
GO   -- GO is acceptable here because we haven't started the main transaction yet

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RoomManagementDB')
    CREATE DATABASE RoomManagementDB;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RatingGHRDB')
    CREATE DATABASE RatingGHRDB;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'HelpDeskGHRDB')
    CREATE DATABASE HelpDeskGHRDB;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LeaveManagementGHRDB')
    CREATE DATABASE LeaveManagementGHRDB;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DutyManagementDB')
    CREATE DATABASE DutyManagementDB;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'IdentityGHRDB')
    CREATE DATABASE IdentityGHRDB;
GO

-- ============================================
-- STEP 2: Single transaction for all schema + data
-- ============================================
BEGIN TRANSACTION;

BEGIN TRY

    -- ==================== DFMDB ====================
    CREATE TABLE [DFMDB].dbo.Facilities (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        Location NVARCHAR(100),
        Department NVARCHAR(100), -- Housekeeping, Maintenance
        Status NVARCHAR(50),       -- Active, Under Maintenance 
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE TABLE [DFMDB].dbo.FacilitySchedules (
        ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
        FacilityId INT NOT NULL,
        DayOfWeek INT NOT NULL, -- 0=Sunday, 1=Monday, ...
        OpenTime TIME NOT NULL,
        CloseTime TIME NOT NULL,
        IsMaintenance BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (FacilityId) REFERENCES [DFMDB].dbo.Facilities(Id)
    );

    CREATE TABLE [DFMDB].dbo.FacilityServices (
        ServiceId INT IDENTITY(1,1) PRIMARY KEY,
        FacilityId INT NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        Price DECIMAL(10,2),
        DurationMinutes INT,
        FOREIGN KEY (FacilityId) REFERENCES [DFMDB].dbo.Facilities(Id)
    );

    CREATE TABLE [DFMDB].dbo.FacilityReservations (
        ReservationId INT IDENTITY(1,1) PRIMARY KEY,
        FacilityId INT NOT NULL,
        ReservedBy NVARCHAR(100) NOT NULL,
        ReservationDate DATE NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        Purpose NVARCHAR(200),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (FacilityId) REFERENCES [DFMDB].dbo.Facilities(Id)
    );

    CREATE TABLE [DFMDB].dbo.FacilityIssues (
        IssueId INT IDENTITY(1,1) PRIMARY KEY,
        FacilityId INT NOT NULL,
        ReportedBy NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Open', -- Open, In Progress, Resolved
        ReportedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        AssignedTo NVARCHAR(100) NULL,
        ResolvedAt DATETIME2 NULL,
        FOREIGN KEY (FacilityId) REFERENCES [DFMDB].dbo.Facilities(Id)
    );

    -- ==================== RoomManagementDB ====================
    CREATE TABLE [RoomManagementDB].dbo.RoomType (
        Id INT IDENTITY PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL,
        Description NVARCHAR(255)
    );

    CREATE TABLE [RoomManagementDB].dbo.Room (
        Id INT IDENTITY PRIMARY KEY,
        RoomNumber NVARCHAR(10) NOT NULL,
        Floor INT NOT NULL,
        TypeId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.RoomType(Id),
        Status NVARCHAR(20) NOT NULL,  -- Available, Occupied, Maintenance, Cleaning
        Description NVARCHAR(255)
    );

    CREATE TABLE [RoomManagementDB].dbo.RoomRate (
        Id INT IDENTITY PRIMARY KEY,
        RoomTypeId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.RoomType(Id),
        PricePerNight DECIMAL(10,2) NOT NULL,
        ValidFrom DATE NOT NULL,
        ValidTo DATE
    );

    CREATE TABLE [RoomManagementDB].dbo.Reservation (
        Id INT IDENTITY PRIMARY KEY,
        GuestId INT NOT NULL, --it is UserId from IdentityServer
        RoomId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.Room(Id),
        CheckInDate DATE NOT NULL,
        CheckOutDate DATE NOT NULL,
        Status NVARCHAR(20) NOT NULL,  -- Pending, Confirmed, Cancelled, CheckedIn, CheckedOut
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );

    CREATE TABLE [RoomManagementDB].dbo.Payment (
        Id INT IDENTITY PRIMARY KEY,
        ReservationId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.Reservation(Id),
        Amount DECIMAL(10,2) NOT NULL,
        PaymentMethod NVARCHAR(20) NOT NULL,
        PaidAt DATETIME NOT NULL DEFAULT GETDATE(),
        TransactionId NVARCHAR(100)
    );

    CREATE TABLE [RoomManagementDB].dbo.CheckIn (
        Id INT IDENTITY PRIMARY KEY,
        ReservationId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.Reservation(Id),
        PerformedByEmployeeId INT NOT NULL,
        Timestamp DATETIME NOT NULL DEFAULT GETDATE()
    );

    CREATE TABLE [RoomManagementDB].dbo.CheckOut (
        Id INT IDENTITY PRIMARY KEY,
        ReservationId INT NOT NULL FOREIGN KEY REFERENCES [RoomManagementDB].dbo.Reservation(Id),
        PerformedByEmployeeId INT NOT NULL,
        Timestamp DATETIME NOT NULL DEFAULT GETDATE()
    );

    -- ==================== RatingGHRDB ====================
    CREATE TABLE [RatingGHRDB].dbo.Departments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL UNIQUE
    );

    CREATE TABLE [RatingGHRDB].dbo.Awards (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UsersId INT NOT NULL,
        DepartmentId INT NOT NULL,
        Title NVARCHAR(100),
        Period VARCHAR(20) CHECK (Period IN ('Weekly', 'Monthly', 'Yearly')),
        Date DATE,
        CONSTRAINT FK_Awards_Department FOREIGN KEY (DepartmentId) REFERENCES [RatingGHRDB].dbo.Departments(Id)
    );

    CREATE TABLE [RatingGHRDB].dbo.Ratings (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        ServiceId INT NOT NULL, --  Fitness Trainer, HR Agent, Help Desk
        DepartmentId INT NOT NULL,
        Stars INT NOT NULL CHECK (Stars BETWEEN 1 AND 10),
        Comment NVARCHAR(1000) NULL,
        RatingDate DATETIME NOT NULL DEFAULT GETDATE(),
        IsApproved BIT NOT NULL DEFAULT 0,
        IsDeleted BIT NOT NULL DEFAULT 0,
        ApprovedAt DATETIME NULL,
        ApprovedBy INT NULL,
        IsFlagged BIT NOT NULL DEFAULT 0,
        FlagReason NVARCHAR(500) NULL,
        CONSTRAINT FK_Ratings_Department FOREIGN KEY (DepartmentId) REFERENCES [RatingGHRDB].dbo.Departments(Id)
    );

    -- ==================== HelpDeskGHRDB ====================
    CREATE TABLE [HelpDeskGHRDB].dbo.Departments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        DepartmentName NVARCHAR(100) UNIQUE NOT NULL -- Rooms, Spa, Casino, Pool, Bar
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Locations (
        Id INT PRIMARY KEY IDENTITY(1,1),
        LocationName NVARCHAR(100) NOT NULL,
        DepartmentId INT NOT NULL,
        FOREIGN KEY (DepartmentId) REFERENCES [HelpDeskGHRDB].dbo.Departments(Id)
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Staff (
        Id INT PRIMARY KEY IDENTITY(1,1),
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) UNIQUE NOT NULL,
        Phone NVARCHAR(20),
        DepartmentId INT,
        Role NVARCHAR(50),
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentId) REFERENCES [HelpDeskGHRDB].dbo.Departments(Id)
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CategoryName NVARCHAR(100) NOT NULL
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Priorities (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PriorityName NVARCHAR(50) NOT NULL
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Statuses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        StatusName NVARCHAR(50) NOT NULL
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.TicketTypes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TypeName NVARCHAR(100) NOT NULL UNIQUE
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.Tickets (
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
        TicketTypeId INT NOT NULL FOREIGN KEY REFERENCES [HelpDeskGHRDB].dbo.TicketTypes(Id),
        CreatedAt DATETIME DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,
        FOREIGN KEY (StaffId) REFERENCES [HelpDeskGHRDB].dbo.Staff(Id),
        FOREIGN KEY (DepartmentId) REFERENCES [HelpDeskGHRDB].dbo.Departments(Id),
        FOREIGN KEY (LocationId) REFERENCES [HelpDeskGHRDB].dbo.Locations(Id),
        FOREIGN KEY (CategoryId) REFERENCES [HelpDeskGHRDB].dbo.Categories(Id),
        FOREIGN KEY (PriorityId) REFERENCES [HelpDeskGHRDB].dbo.Priorities(Id),
        FOREIGN KEY (StatusId) REFERENCES [HelpDeskGHRDB].dbo.Statuses(Id)
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.TicketLogs (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TicketId INT NOT NULL,
        Comment NVARCHAR(1000),
        CreatedBy INT NOT NULL, --UserId
        CreatedByRole NVARCHAR(10),
        CreatedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (TicketId) REFERENCES [HelpDeskGHRDB].dbo.Tickets(Id)
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.TicketRatings (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TicketId INT NOT NULL,
        RatedByUserId NVARCHAR(450) NOT NULL,
        Score INT NOT NULL CHECK (Score >= 1 AND Score <= 5),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        FOREIGN KEY (TicketId) REFERENCES [HelpDeskGHRDB].dbo.Tickets(Id) ON DELETE CASCADE
    );

    CREATE TABLE [HelpDeskGHRDB].dbo.TicketComments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TicketId INT NOT NULL,
        Text NVARCHAR(1000) NOT NULL,
        CreatedByUserId INT NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_TicketComments_Tickets FOREIGN KEY (TicketId) REFERENCES [HelpDeskGHRDB].dbo.Tickets(Id)
    );

    -- ==================== LeaveManagementGHRDB ====================
    CREATE TABLE [LeaveManagementGHRDB].dbo.LeaveTypes (
        Id INT PRIMARY KEY IDENTITY,
        Name NVARCHAR(100) NOT NULL,       
        Description NVARCHAR(255),
        MaxDaysPerYear INT NOT NULL
    );

    CREATE TABLE [LeaveManagementGHRDB].dbo.LeaveBalances (
        Id INT PRIMARY KEY IDENTITY,
        UserId INT NOT NULL,
        LeaveTypeId INT NOT NULL,
        RemainingDays DECIMAL(5,2) NOT NULL,
        LastUpdated DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (LeaveTypeId) REFERENCES [LeaveManagementGHRDB].dbo.LeaveTypes(Id),
        CONSTRAINT UC_LeaveBalance UNIQUE(UserId, LeaveTypeId)   -- Fixed: was (UserId, Id)
    );

    CREATE TABLE [LeaveManagementGHRDB].dbo.LeaveApplications (
        Id INT PRIMARY KEY IDENTITY,
        UserId INT NOT NULL,
        FullName NVARCHAR(1000),
        PhoneNumber NVARCHAR(10),
        Email NVARCHAR(30),
        Department INT NOT NULL,
        LeaveTypeId INT NOT NULL,
        StartDate DATE NOT NULL,
        EndDate DATE NOT NULL,
        TotalDays DECIMAL(5,2) NOT NULL,
        Reason NVARCHAR(500),
        Status NVARCHAR(20) DEFAULT 'Pending',
        ApproverId INT,
        DecisionDate DATETIME,
        RequestedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (LeaveTypeId) REFERENCES [LeaveManagementGHRDB].dbo.LeaveTypes(Id)
    );

    CREATE TABLE [LeaveManagementGHRDB].dbo.Holidays (
        Id INT PRIMARY KEY IDENTITY,
        Name NVARCHAR(100) NOT NULL,
        Date DATE NOT NULL,
        IsRecurring BIT DEFAULT 0
    );

    -- ==================== DutyManagementDB ====================
    CREATE TABLE [DutyManagementDB].dbo.Duties (
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

    CREATE TABLE [DutyManagementDB].dbo.Shifts (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL
    );

    CREATE TABLE [DutyManagementDB].dbo.PeriodType (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL
    );

    CREATE TABLE [DutyManagementDB].dbo.DutyAssignments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeId INT NOT NULL,
        PeriodTypeId INT NOT NULL,
        DutyId INT NOT NULL,
        ShiftId INT NOT NULL,
        AssignmentDate DATE NOT NULL,
        FOREIGN KEY (PeriodTypeId) REFERENCES [DutyManagementDB].dbo.PeriodType(Id),
        FOREIGN KEY (DutyId) REFERENCES [DutyManagementDB].dbo.Duties(Id),
        FOREIGN KEY (ShiftId) REFERENCES [DutyManagementDB].dbo.Shifts(Id)
    );

    -- ==================== IdentityGHRDB ====================
    CREATE TABLE [IdentityGHRDB].dbo.Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(255) NOT NULL UNIQUE,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        PhoneNumber NVARCHAR(256),
        DateCreated DATETIME DEFAULT GETDATE()
    );

    CREATE TABLE [IdentityGHRDB].dbo.Roles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL UNIQUE,
        Description NVARCHAR(500),
        DateCreated DATETIME DEFAULT GETDATE()
    );

    CREATE TABLE [IdentityGHRDB].dbo.UserRoles (
        UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        FOREIGN KEY (UserId) REFERENCES [IdentityGHRDB].dbo.Users(Id) ON DELETE CASCADE,
        FOREIGN KEY (RoleId) REFERENCES [IdentityGHRDB].dbo.Roles(Id) ON DELETE CASCADE,
        CONSTRAINT UC_UserRole UNIQUE (UserId, RoleId)
    );

    -- ============================ DATA INSERTS ============================

    -- -------------------- RoomManagementDB --------------------
    INSERT INTO [RoomManagementDB].dbo.RoomType (Name, Description)
    VALUES 
        ('Standard', 'Standard room with basic amenities'),
        ('Deluxe', 'Spacious deluxe room with sea view'),
        ('Suite', 'Luxury suite with separate living area');

    INSERT INTO [RoomManagementDB].dbo.Room (RoomNumber, Floor, TypeId, Status, Description)
    VALUES 
        ('101', 1, 1, 'Available', 'Standard room'),
        ('102', 1, 2, 'Cleaning', 'Deluxe room sea-facing'),
        ('201', 2, 3, 'Occupied', 'Luxury suite');

    INSERT INTO [RoomManagementDB].dbo.RoomRate (RoomTypeId, PricePerNight, ValidFrom, ValidTo)
    VALUES 
        (1, 100.00, '2025-01-01', NULL),
        (2, 150.00, '2025-01-01', NULL),
        (3, 250.00, '2025-01-01', NULL);

    INSERT INTO [RoomManagementDB].dbo.Reservation (GuestId, RoomId, CheckInDate, CheckOutDate, Status)
    VALUES 
        (1, 1, '2025-06-10', '2025-06-15', 'Confirmed'),
        (1, 2, '2025-07-01', '2025-07-05', 'Pending');

    INSERT INTO [RoomManagementDB].dbo.Payment (ReservationId, Amount, PaymentMethod, TransactionId)
    VALUES 
        (1, 500.00, 'CreditCard', 'TXN123456789'),
        (2, 300.00, 'PayPal', 'TXN987654321');

    INSERT INTO [RoomManagementDB].dbo.CheckIn (ReservationId, PerformedByEmployeeId)
    VALUES (1, 1);

    INSERT INTO [RoomManagementDB].dbo.CheckOut (ReservationId, PerformedByEmployeeId)
    VALUES (1, 1);

    -- -------------------- RatingGHRDB --------------------
    INSERT INTO [RatingGHRDB].dbo.Departments (Name)
    VALUES 
        ('Bar'), ('Casino'), ('Disco Club'), ('Duty'),
        ('Fitness & Spa'), ('Help Desk'), ('HR Platform'), ('Restaurant');

    -- (Awards and Ratings have no initial data)

    -- -------------------- HelpDeskGHRDB --------------------
    INSERT INTO [HelpDeskGHRDB].dbo.Departments (DepartmentName) VALUES 
        ('Rooms'), ('Spa'), ('Casino'), ('Pool'), ('Bar'), ('Hotel'), ('Disco'), ('Beach'), ('Shop');

    INSERT INTO [HelpDeskGHRDB].dbo.Locations (LocationName, DepartmentId) VALUES 
        ('Hotel', 1), ('Spa center', 2), ('Casino', 3), ('Big Pool', 4);

    INSERT INTO [HelpDeskGHRDB].dbo.Staff (FullName, Email, Phone, DepartmentId, Role, IsActive) VALUES 
        ('Sekul Kamberov', 'sekul1@gmail.com', '+123456789', 1, 'Staff', 1);   -- Role changed from 1 to 'Staff'

    INSERT INTO [HelpDeskGHRDB].dbo.Categories (CategoryName) VALUES 
        ('Hotel Rooms'), ('Spa center'), ('Casino'), ('Big Pool');

    INSERT INTO [HelpDeskGHRDB].dbo.Priorities (PriorityName) VALUES 
        ('Critical'), ('High'), ('Normal'), ('Low');

    INSERT INTO [HelpDeskGHRDB].dbo.Statuses (StatusName) VALUES 
        ('Open'), ('In Progress'), ('On Hold'), ('Resolved'), ('Closed'), ('Escalated');

    INSERT INTO [HelpDeskGHRDB].dbo.TicketTypes (TypeName) VALUES
        ('Room Service'), ('Housekeeping'), ('Maintenance'), ('Front Desk'), ('Billing'), ('Other');

    -- Seed 100 tickets
    DECLARE @i INT = 0;
    WHILE @i < 100
    BEGIN
        INSERT INTO [HelpDeskGHRDB].dbo.Tickets (
            Title, Description, UserId, StaffId, DepartmentId, LocationId,
            CategoryId, PriorityId, StatusId, TicketTypeId, CreatedAt
        )
        SELECT
            'Test Ticket ' + CAST(@i AS NVARCHAR(10)),
            'Random Description for ticket #' + CAST(@i AS NVARCHAR(10)),
            ABS(CHECKSUM(NEWID())) % 10 + 1,   -- UserId 1-10
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Staff ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Departments ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Locations ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Categories ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Priorities ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.Statuses ORDER BY NEWID()),
            (SELECT TOP 1 Id FROM [HelpDeskGHRDB].dbo.TicketTypes ORDER BY NEWID()),
            DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE());
        SET @i = @i + 1;
    END

    -- -------------------- LeaveManagementGHRDB --------------------
    INSERT INTO [LeaveManagementGHRDB].dbo.LeaveTypes (Name, Description, MaxDaysPerYear)
    VALUES 
        ('Annual', 'Annual Leave: Paid Time Off (PTO). Purpose: Vacation, rest, or personal time.', 25),
        ('Sick', 'Sick Leave Purpose: For illness, injury, or medical appointments. Paid: Often yes, but policies vary.', 25),
        ('Maternity', 'Purpose: For childbirth and postnatal recovery. Paid: Varies by country/employer. Duration: Usually ranges from 12 weeks to 6 months.', 182);   -- removed trailing comma

    -- -------------------- DutyManagementDB --------------------
    INSERT INTO [DutyManagementDB].dbo.Shifts (Name) VALUES ('Morning'), ('Afternoon'), ('Night');
    INSERT INTO [DutyManagementDB].dbo.PeriodType (Name) VALUES ('Week'), ('Month'), ('Day');

    -- -------------------- IdentityGHRDB --------------------
    SET IDENTITY_INSERT [IdentityGHRDB].dbo.Roles ON;
    INSERT INTO [IdentityGHRDB].dbo.Roles (Id, Name, Description)
    VALUES 
        (1, 'EMPLOYEE', 'Basic employee role'),
        (2, 'MANAGER', 'Manager role'),
        (3, 'HR ADMIN', 'Human Resources Admin role'),
        (4, 'HOUSEKEEPER', 'Room Attendant'),
        (5, 'HOUSEKEEPER MANAGER', 'Room Attendant Manager'),
        (6, 'HOTEL GUEST', 'HOTEL GUEST is a client');
    SET IDENTITY_INSERT [IdentityGHRDB].dbo.Roles OFF;

    INSERT INTO [IdentityGHRDB].dbo.Users (Username, Email, PasswordHash, PhoneNumber)
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
        ('Sekul12', 'sekul12@gmail.com', 'KZtJqHCbyLkkeeXMYALGtw==;r0mvP0GBsM7wQGVO2V7NG+8SQa0otXZ7gR9S3IbL5EI=', '+123456787');

    -- ============================ COMMIT ============================
    COMMIT TRANSACTION;
    PRINT 'All operations completed successfully.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO