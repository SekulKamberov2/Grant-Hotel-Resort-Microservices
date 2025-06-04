 ## SQL

 ```sql
 
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'RoomManagementDB'
)
BEGIN
    CREATE DATABASE RoomManagementDB;
END


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