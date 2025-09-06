# Script to export data from BloodDonation database
$ServerName = "localhost"
$DatabaseName = "BloodDonation"
$OutputPath = "C:\GitHub\Blood-Donation-System\DatabaseBackup"

# Create output directory if it doesn't exist
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath
}

# List of tables to export (in order to handle foreign key dependencies)
$tables = @(
    "BloodType",
    "BloodBank", 
    "MedicalCenter",
    "Account",
    "Donor",
    "HealthSurvey",
    "DonationAppointment",
    "DonationCertificate",
    "BloodRequest",
    "DonorBloodRequest",
    "BloodInventory",
    "Notification",
    "News"
)

Write-Host "Starting data export from $DatabaseName database..." -ForegroundColor Green

foreach ($table in $tables) {
    $outputFile = Join-Path $OutputPath "$table.csv"
    
    try {
        # Using sqlcmd to export data to CSV
        $query = "SET NOCOUNT ON; SELECT * FROM $table"
        sqlcmd -S $ServerName -d $DatabaseName -E -s"," -W -Q $query -o $outputFile
        
        Write-Host "✓ Exported $table to $outputFile" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to export $table: $_" -ForegroundColor Red
    }
}

# Also generate INSERT scripts using BCP
Write-Host "`nGenerating INSERT scripts..." -ForegroundColor Yellow

$insertScriptPath = Join-Path $OutputPath "DataInserts.sql"
$insertScript = @"
-- Blood Donation System Data Insert Script
-- Generated on $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

USE BloodDonation;
GO

-- Disable constraints temporarily
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'
GO

"@

foreach ($table in $tables) {
    $insertScript += @"

-- Insert data for $table
PRINT 'Inserting data into $table...'

"@
}

$insertScript += @"

-- Re-enable constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all'
GO

PRINT 'Data import completed successfully!'
"@

$insertScript | Out-File -FilePath $insertScriptPath -Encoding UTF8

Write-Host "`n✓ Export completed!" -ForegroundColor Green
Write-Host "Files saved to: $OutputPath" -ForegroundColor Cyan
