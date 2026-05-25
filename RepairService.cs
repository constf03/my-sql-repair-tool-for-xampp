namespace MySQLRepairTool
{
    public class RepairService
    {
        private static void ShowSuccessDialog()
        {
            MessageBox.Show(
                "Successfully repaired XAMPP MySQL!",
                "Repair Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(
                message,
                "Repair Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    $"Source directory not found: {sourceDir}");

            Directory.CreateDirectory(destinationDir);

            // Copy files
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);

                // overwrite existing files
                file.CopyTo(targetFilePath, true);
            }

            // Copy subdirectories
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    string newDestinationDir =
                        Path.Combine(destinationDir, subDir.Name);

                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public void Repair(string mysqlPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mysqlPath))
                {
                    ShowErrorDialog("Invalid MySQL path.");
                    return;
                }

                if (!mysqlPath
                    .ToLower()
                    .Contains("xampp", StringComparison.CurrentCultureIgnoreCase))
                {
                    ShowErrorDialog("Selected folder is not a XAMPP directory.");
                    return;
                }

                string backupPath = Path.Combine(mysqlPath, "backup");
                string dataPath = Path.Combine(mysqlPath, "data");

                if (!Directory.Exists(backupPath))
                {
                    ShowErrorDialog("Backup folder not found.");
                    return;
                }

                if (!Directory.Exists(dataPath))
                {
                    ShowErrorDialog("Data folder not found.");
                    return;
                }

                // =========================
                // DELETE OLD FILES
                // =========================
                foreach (string file in Directory.GetFiles(dataPath))
                {
                    string fileName = Path.GetFileName(file);

                    // keep ibdata1
                    if (!fileName.Equals("ibdata1", StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(file);
                    }
                }

                // =========================
                // DELETE OLD DIRECTORIES
                // =========================
                foreach (string dir in Directory.GetDirectories(dataPath))
                {
                    string dirName = Path.GetFileName(dir);
                    string[] dirsToTransferNames = [
                            "mysql",
                            "performance_schema",
                            "phpmyadmin",
                            "test"
                        ];

                    for (int i = 0; i < dirsToTransferNames.Length; i++)
                    {
                        if (dirName.Equals(dirsToTransferNames[i], StringComparison.OrdinalIgnoreCase))
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                }

                // =========================
                // COPY BACKUP FILES
                // =========================
                foreach (string file in Directory.GetFiles(backupPath))
                {
                    string fileName = Path.GetFileName(file);

                    if (!fileName.Equals("ibdata1", StringComparison.OrdinalIgnoreCase))
                    {
                        string destFile =
                            Path.Combine(dataPath, fileName);

                        File.Copy(file, destFile, true);
                    }
                }

                // =========================
                // COPY BACKUP DIRECTORIES
                // =========================
                foreach (string dir in Directory.GetDirectories(backupPath))
                {
                    string dirName = Path.GetFileName(dir);

                    string destDir =
                        Path.Combine(dataPath, dirName);

                    CopyDirectory(dir, destDir, true);
                }

                ShowSuccessDialog();
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
    }
}