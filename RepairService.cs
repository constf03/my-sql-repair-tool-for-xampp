using System;
using System.IO;
using System.Windows.Forms;

namespace MySQLRepairTool
{
    public interface IDialogService
    {
        void ShowSuccess();
        void ShowError(string message);
    }

    public class WinFormsDialogService : IDialogService
    {
        public void ShowSuccess()
        {
            MessageBox.Show(
                "Successfully repaired XAMPP MySQL!",
                "Repair Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Repair Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    public class RepairService
    {
        private readonly IDialogService _dialogService;

        public RepairService(IDialogService? dialogService = null)
        {
            _dialogService = dialogService ?? new WinFormsDialogService();
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
                    _dialogService.ShowError("Invalid MySQL path.");
                    return;
                }

                if (!mysqlPath.Contains("xampp", StringComparison.OrdinalIgnoreCase))
                {
                    _dialogService.ShowError("Selected folder is not a XAMPP directory.");
                    return;
                }

                string backupPath = Path.Combine(mysqlPath, "backup");
                string dataPath = Path.Combine(mysqlPath, "data");
                string dataOldPath = Path.Combine(mysqlPath, "data_old");

                if (!Directory.Exists(backupPath))
                {
                    _dialogService.ShowError("Backup folder not found.");
                    return;
                }

                if (!Directory.Exists(dataPath))
                {
                    _dialogService.ShowError("Data folder not found.");
                    return;
                }

                // =========================
                // MAKE A COPY OF DATA FOLDER
                // =========================
                if (Directory.Exists(dataOldPath))
                {
                    Directory.Delete(dataOldPath, true);
                }
                CopyDirectory(dataPath, dataOldPath, true);

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
                    string[] dirsToTransferNames = new[] {
                        "mysql",
                        "performance_schema",
                        "phpmyadmin",
                        "test"
                    };

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

                _dialogService.ShowSuccess();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }
    }
}