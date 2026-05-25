# Root directory
$root = "XAMPPTestDirectory"

# If the root directory already exists, delete it completely
if (Test-Path $root) {
    Remove-Item $root -Recurse -Force
}

# List of directories to create
$directories = @(
    "XAMPPTestDirectory/xampp/mysql/backup/mysql",
    "XAMPPTestDirectory/xampp/mysql/backup/preformance_shema",
    "XAMPPTestDirectory/xampp/mysql/backup/phpmyadmin",
    "XAMPPTestDirectory/xampp/mysql/backup/test",
    "XAMPPTestDirectory/xampp/mysql/data/dummydir01",
    "XAMPPTestDirectory/xampp/mysql/data/dummydir02",
    "XAMPPTestDirectory/xampp/mysql/data/mysql",
    "XAMPPTestDirectory/xampp/mysql/data/preformance_shema",
    "XAMPPTestDirectory/xampp/mysql/data/phpmyadmin",
    "XAMPPTestDirectory/xampp/mysql/data/test"
)

# Create directories
foreach ($dir in $directories) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
}

# List of files to create
$files = @(
    "XAMPPTestDirectory/xampp/mysql/backup/ibdata1",
    "XAMPPTestDirectory/xampp/mysql/data/ibdata1",
    "XAMPPTestDirectory/xampp/mysql/backup/dummyfile01",
    "XAMPPTestDirectory/xampp/mysql/backup/dummyfile02",
    "XAMPPTestDirectory/xampp/mysql/data/dummyfile01",
    "XAMPPTestDirectory/xampp/mysql/data/dummyfile02",
    "XAMPPTestDirectory/xampp/mysql/backup/mysql/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/backup/preformance_shema/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/backup/phpmyadmin/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/backup/test/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/dummydir01/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/dummydir02/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/mysql/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/preformance_shema/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/phpmyadmin/dummyfile",
    "XAMPPTestDirectory/xampp/mysql/data/test/dummyfile"
)

# Create files
foreach ($file in $files) {
    New-Item -ItemType File -Path $file -Force | Out-Null
}

Write-Host "Directory tree and files created successfully."