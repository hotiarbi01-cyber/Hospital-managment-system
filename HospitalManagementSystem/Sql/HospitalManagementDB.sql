-- 1) Create Database
IF DB_ID('HospitalManagementDB') IS NULL
BEGIN
    CREATE DATABASE HospitalManagementDB;
END
GO

USE HospitalManagementDB;
GO

-- 2) Drop old tables (for easy reset in school/lab environment)
IF OBJECT_ID('MedicalRecords', 'U') IS NOT NULL DROP TABLE MedicalRecords;
IF OBJECT_ID('Bills', 'U') IS NOT NULL DROP TABLE Bills;
IF OBJECT_ID('Appointments', 'U') IS NOT NULL DROP TABLE Appointments;
IF OBJECT_ID('Rooms', 'U') IS NOT NULL DROP TABLE Rooms;
IF OBJECT_ID('Doctors', 'U') IS NOT NULL DROP TABLE Doctors;
IF OBJECT_ID('Patients', 'U') IS NOT NULL DROP TABLE Patients;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- 3) Tables
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Doctor', 'Receptionist')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Patients (
    PatientId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Address NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Doctors (
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Email NVARCHAR(120) NULL,
    WorkSchedule NVARCHAR(120) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Appointments (
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentDateTime DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'Completed', 'Cancelled')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE CASCADE,
    CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId) ON DELETE CASCADE
);

CREATE TABLE MedicalRecords (
    RecordId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NOT NULL,
    Diagnosis NVARCHAR(255) NOT NULL,
    Symptoms NVARCHAR(500) NULL,
    Medicines NVARCHAR(500) NULL,
    DoctorNotes NVARCHAR(1000) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_MedicalRecords_Appointments FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId) ON DELETE CASCADE
);

CREATE TABLE Bills (
    BillId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    ServiceName NVARCHAR(120) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Paid', 'Unpaid')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Bills_Patients FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE CASCADE
);

CREATE TABLE Rooms (
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(20) NOT NULL UNIQUE,
    RoomType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Available', 'Occupied')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 4) Stored Procedures (examples for CRUD)
CREATE OR ALTER PROCEDURE sp_AddPatient
    @FullName NVARCHAR(120),
    @DateOfBirth DATE,
    @Phone NVARCHAR(20),
    @Address NVARCHAR(255)
AS
BEGIN
    INSERT INTO Patients (FullName, DateOfBirth, Phone, Address)
    VALUES (@FullName, @DateOfBirth, @Phone, @Address);
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateBillStatus
    @BillId INT,
    @Status NVARCHAR(20)
AS
BEGIN
    UPDATE Bills
    SET Status = @Status
    WHERE BillId = @BillId;
END
GO

-- 5) Sample Data
INSERT INTO Users (Username, PasswordHash, Role)
VALUES
('admin', 'admin123', 'Admin'),
('doctor1', 'doctor123', 'Doctor'),
('reception', 'recep123', 'Receptionist');

INSERT INTO Patients (FullName, DateOfBirth, Phone, Address)
VALUES
('Ardit Hoxha', '2000-05-12', '0691111111', 'Tirane'),
('Era Kola', '1995-11-03', '0692222222', 'Durres'),
('Blerim Meta', '1987-02-19', '0693333333', 'Vlore');

INSERT INTO Doctors (FullName, Specialization, Phone, Email, WorkSchedule)
VALUES
('Dr. Sara Dervishi', 'Cardiology', '0681111111', 'sara@hospital.local', '08:00-14:00'),
('Dr. Gent Leka', 'Neurology', '0682222222', 'gent@hospital.local', '10:00-16:00');

INSERT INTO Appointments (PatientId, DoctorId, AppointmentDateTime, Status)
VALUES
(1, 1, DATEADD(HOUR, 2, GETDATE()), 'Pending'),
(2, 2, DATEADD(DAY, -1, GETDATE()), 'Completed');

INSERT INTO MedicalRecords (AppointmentId, Diagnosis, Symptoms, Medicines, DoctorNotes)
VALUES
(2, 'Migraine', 'Headache, nausea', 'Ibuprofen', 'Pushim dhe hidratim i mire.');

INSERT INTO Bills (PatientId, ServiceName, Price, Status)
VALUES
(1, 'General Checkup', 30.00, 'Unpaid'),
(2, 'Neurology Consultation', 75.00, 'Paid');

INSERT INTO Rooms (RoomNumber, RoomType, Status)
VALUES
('101', 'Single', 'Available'),
('202', 'Double', 'Occupied');
GO
