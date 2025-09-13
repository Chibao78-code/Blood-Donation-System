IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [BloodBank] (
    [BloodBankID] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Location] nvarchar(255) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_BloodBank] PRIMARY KEY ([BloodBankID])
);

CREATE TABLE [BloodType] (
    [BloodTypeID] int NOT NULL IDENTITY,
    [Type] nvarchar(10) NOT NULL,
    CONSTRAINT [PK_BloodType] PRIMARY KEY ([BloodTypeID])
);

CREATE TABLE [News] (
    [NewsId] int NOT NULL IDENTITY,
    [Title] nvarchar(255) NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Type] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_News] PRIMARY KEY ([NewsId])
);

CREATE TABLE [MedicalCenter] (
    [MedicalCenterID] int NOT NULL IDENTITY,
    [Name] nvarchar(255) NOT NULL,
    [Location] nvarchar(255) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [BloodBankID] int NOT NULL,
    CONSTRAINT [PK_MedicalCenter] PRIMARY KEY ([MedicalCenterID]),
    CONSTRAINT [FK_MedicalCenter_BloodBank_BloodBankID] FOREIGN KEY ([BloodBankID]) REFERENCES [BloodBank] ([BloodBankID]) ON DELETE CASCADE
);

CREATE TABLE [BloodInventory] (
    [InventoryID] int NOT NULL IDENTITY,
    [BloodTypeID] int NOT NULL,
    [BloodBankID] int NOT NULL,
    [Quantity] decimal(10,2) NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_BloodInventory] PRIMARY KEY ([InventoryID]),
    CONSTRAINT [FK_BloodInventory_BloodBank_BloodBankID] FOREIGN KEY ([BloodBankID]) REFERENCES [BloodBank] ([BloodBankID]) ON DELETE CASCADE,
    CONSTRAINT [FK_BloodInventory_BloodType_BloodTypeID] FOREIGN KEY ([BloodTypeID]) REFERENCES [BloodType] ([BloodTypeID]) ON DELETE CASCADE
);

CREATE TABLE [Account] (
    [AccountID] int NOT NULL IDENTITY,
    [MedicalCenterID] int NULL,
    [Username] nvarchar(50) NOT NULL,
    [Password] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [PermissionLevel] int NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY ([AccountID]),
    CONSTRAINT [FK_Account_MedicalCenter_MedicalCenterID] FOREIGN KEY ([MedicalCenterID]) REFERENCES [MedicalCenter] ([MedicalCenterID])
);

CREATE TABLE [BloodRequest] (
    [BloodRequestID] int NOT NULL IDENTITY,
    [MedicalCenterID] int NOT NULL,
    [BloodTypeID] int NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [Quantity] DECIMAL(10,2) NOT NULL,
    [BloodGiven] NVARCHAR(50) NULL,
    [IsEmergency] bit NOT NULL,
    [IsCompatible] bit NOT NULL,
    [Status] nvarchar(max) NULL,
    CONSTRAINT [PK_BloodRequest] PRIMARY KEY ([BloodRequestID]),
    CONSTRAINT [FK_BloodRequest_BloodType_BloodTypeID] FOREIGN KEY ([BloodTypeID]) REFERENCES [BloodType] ([BloodTypeID]) ON DELETE CASCADE,
    CONSTRAINT [FK_BloodRequest_MedicalCenter_MedicalCenterID] FOREIGN KEY ([MedicalCenterID]) REFERENCES [MedicalCenter] ([MedicalCenterID]) ON DELETE CASCADE
);

CREATE TABLE [Donor] (
    [DonorID] int NOT NULL IDENTITY,
    [AccountID] int NOT NULL,
    [BloodTypeID] int NULL,
    [Name] nvarchar(255) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Gender] nvarchar(10) NULL,
    [Address] nvarchar(255) NULL,
    [IsAvailable] bit NULL,
    [CCCD] nvarchar(50) NULL,
    CONSTRAINT [PK_Donor] PRIMARY KEY ([DonorID]),
    CONSTRAINT [FK_Donor_Account_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [Account] ([AccountID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Donor_BloodType_BloodTypeID] FOREIGN KEY ([BloodTypeID]) REFERENCES [BloodType] ([BloodTypeID])
);

CREATE TABLE [DonationAppointment] (
    [AppointmentID] int NOT NULL IDENTITY,
    [DonorID] int NOT NULL,
    [MedicalCenterID] int NOT NULL,
    [TimeSlot] nvarchar(10) NOT NULL,
    [BloodTypeID] int NOT NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    [QuantityDonated] decimal(18,2) NOT NULL,
    [HealthSurvey] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_DonationAppointment] PRIMARY KEY ([AppointmentID]),
    CONSTRAINT [FK_DonationAppointment_BloodType_BloodTypeID] FOREIGN KEY ([BloodTypeID]) REFERENCES [BloodType] ([BloodTypeID]) ON DELETE CASCADE,
    CONSTRAINT [FK_DonationAppointment_Donor_DonorID] FOREIGN KEY ([DonorID]) REFERENCES [Donor] ([DonorID]) ON DELETE CASCADE,
    CONSTRAINT [FK_DonationAppointment_MedicalCenter_MedicalCenterID] FOREIGN KEY ([MedicalCenterID]) REFERENCES [MedicalCenter] ([MedicalCenterID]) ON DELETE CASCADE
);

CREATE TABLE [DonorBloodRequest] (
    [DonorBloodRequestID] int NOT NULL IDENTITY,
    [BloodRequestID] int NOT NULL,
    [DonorID] int NOT NULL,
    [DonationDate] datetime2 NULL,
    [QuantityDonated] decimal(10,2) NULL,
    CONSTRAINT [PK_DonorBloodRequest] PRIMARY KEY ([DonorBloodRequestID]),
    CONSTRAINT [FK_DonorBloodRequest_BloodRequest_BloodRequestID] FOREIGN KEY ([BloodRequestID]) REFERENCES [BloodRequest] ([BloodRequestID]) ON DELETE CASCADE,
    CONSTRAINT [FK_DonorBloodRequest_Donor_DonorID] FOREIGN KEY ([DonorID]) REFERENCES [Donor] ([DonorID]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [NotificationID] int NOT NULL IDENTITY,
    [DonorID] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [Type] nvarchar(50) NULL,
    [IsConfirmed] bit NOT NULL,
    [AccountID] int NULL,
    [BloodRequestID] int NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationID]),
    CONSTRAINT [FK_Notifications_Account_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [Account] ([AccountID]),
    CONSTRAINT [FK_Notifications_BloodRequest_BloodRequestID] FOREIGN KEY ([BloodRequestID]) REFERENCES [BloodRequest] ([BloodRequestID]),
    CONSTRAINT [FK_Notifications_Donor_DonorID] FOREIGN KEY ([DonorID]) REFERENCES [Donor] ([DonorID]) ON DELETE CASCADE
);

CREATE TABLE [DonationCertificate] (
    [CertificateID] int NOT NULL IDENTITY,
    [AppointmentID] int NOT NULL,
    [IssueDate] datetime2 NOT NULL,
    [CertificateDetails] nvarchar(250) NOT NULL,
    CONSTRAINT [PK_DonationCertificate] PRIMARY KEY ([CertificateID]),
    CONSTRAINT [FK_DonationCertificate_DonationAppointment_AppointmentID] FOREIGN KEY ([AppointmentID]) REFERENCES [DonationAppointment] ([AppointmentID]) ON DELETE CASCADE
);

CREATE TABLE [HealthSurvey] (
    [SurveyID] int NOT NULL IDENTITY,
    [AppointmentID] int NOT NULL,
    [QuestionCode] nvarchar(500) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_HealthSurvey] PRIMARY KEY ([SurveyID]),
    CONSTRAINT [FK_HealthSurvey_DonationAppointment_AppointmentID] FOREIGN KEY ([AppointmentID]) REFERENCES [DonationAppointment] ([AppointmentID]) ON DELETE CASCADE
);

CREATE INDEX [IX_Account_MedicalCenterID] ON [Account] ([MedicalCenterID]);

CREATE INDEX [IX_BloodInventory_BloodBankID] ON [BloodInventory] ([BloodBankID]);

CREATE INDEX [IX_BloodInventory_BloodTypeID] ON [BloodInventory] ([BloodTypeID]);

CREATE INDEX [IX_BloodRequest_BloodTypeID] ON [BloodRequest] ([BloodTypeID]);

CREATE INDEX [IX_BloodRequest_MedicalCenterID] ON [BloodRequest] ([MedicalCenterID]);

CREATE INDEX [IX_DonationAppointment_BloodTypeID] ON [DonationAppointment] ([BloodTypeID]);

CREATE INDEX [IX_DonationAppointment_DonorID] ON [DonationAppointment] ([DonorID]);

CREATE INDEX [IX_DonationAppointment_MedicalCenterID] ON [DonationAppointment] ([MedicalCenterID]);

CREATE UNIQUE INDEX [IX_DonationCertificate_AppointmentID] ON [DonationCertificate] ([AppointmentID]);

CREATE UNIQUE INDEX [IX_Donor_AccountID] ON [Donor] ([AccountID]);

CREATE INDEX [IX_Donor_BloodTypeID] ON [Donor] ([BloodTypeID]);

CREATE INDEX [IX_DonorBloodRequest_BloodRequestID] ON [DonorBloodRequest] ([BloodRequestID]);

CREATE INDEX [IX_DonorBloodRequest_DonorID] ON [DonorBloodRequest] ([DonorID]);

CREATE INDEX [IX_HealthSurvey_AppointmentID] ON [HealthSurvey] ([AppointmentID]);

CREATE INDEX [IX_MedicalCenter_BloodBankID] ON [MedicalCenter] ([BloodBankID]);

CREATE INDEX [IX_Notifications_AccountID] ON [Notifications] ([AccountID]);

CREATE INDEX [IX_Notifications_BloodRequestID] ON [Notifications] ([BloodRequestID]);

CREATE INDEX [IX_Notifications_DonorID] ON [Notifications] ([DonorID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250905084634_InitialCreate', N'9.0.6');

COMMIT;
GO

