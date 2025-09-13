USE BloodDonationDB;
GO

-- Insert Blood Types
IF NOT EXISTS (SELECT * FROM BloodTypes)
BEGIN
    INSERT INTO BloodTypes (Type, Description, CreatedAt, IsDeleted) VALUES
    ('A+', 'Nhóm máu A dương', GETDATE(), 0),
    ('A-', 'Nhóm máu A âm', GETDATE(), 0),
    ('B+', 'Nhóm máu B dương', GETDATE(), 0),
    ('B-', 'Nhóm máu B âm', GETDATE(), 0),
    ('O+', 'Nhóm máu O dương', GETDATE(), 0),
    ('O-', 'Nhóm máu O âm', GETDATE(), 0),
    ('AB+', 'Nhóm máu AB dương', GETDATE(), 0),
    ('AB-', 'Nhóm máu AB âm', GETDATE(), 0);
END

-- Insert Medical Centers
IF NOT EXISTS (SELECT * FROM MedicalCenters)
BEGIN
    INSERT INTO MedicalCenters (Name, Address, City, District, PhoneNumber, Email, IsActive, CreatedAt, IsDeleted) VALUES
    ('Bệnh viện Chợ Rẫy', '201B Nguyễn Chí Thanh', 'TP.HCM', 'Quận 5', '02838554137', 'choray@gmail.com', 1, GETDATE(), 0),
    ('Bệnh viện Nhân dân 115', '527 Sư Vạn Hạnh', 'TP.HCM', 'Quận 10', '02838654387', 'bv115@gmail.com', 1, GETDATE(), 0),
    ('Bệnh viện Truyền máu Huyết học', '118 Hồng Bàng', 'TP.HCM', 'Quận 5', '02838554858', 'btmhh@gmail.com', 1, GETDATE(), 0);
END

PRINT 'Seed data inserted successfully!';
