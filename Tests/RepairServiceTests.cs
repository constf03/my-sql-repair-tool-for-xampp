using System;
using System.IO;
using Xunit;
using MySQLRepairTool;

namespace MySQLRepairTool.Tests
{
    public class FakeDialogService : IDialogService
    {
        public bool SuccessCalled { get; private set; }
        public string? LastError { get; private set; }

        public void ShowSuccess() => SuccessCalled = true;
        public void ShowError(string message) => LastError = message;
    }

    public class RepairServiceTests : IDisposable
    {
        private readonly string _root;
        private readonly string _mysqlPath;
        private readonly string _backupPath;
        private readonly string _dataPath;
        private readonly FakeDialogService _dialog;

        public RepairServiceTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "MySQLRepairTool_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);

            // ensure mysqlPath contains "xampp" to pass validation
            _mysqlPath = Path.Combine(_root, "xampp");
            _backupPath = Path.Combine(_mysqlPath, "backup");
            _dataPath = Path.Combine(_mysqlPath, "data");

            Directory.CreateDirectory(_backupPath);
            Directory.CreateDirectory(_dataPath);

            // create backup file and directory
            File.WriteAllText(Path.Combine(_backupPath, "schema.frm"), "backup-schema");
            Directory.CreateDirectory(Path.Combine(_backupPath, "mysql"));
            File.WriteAllText(Path.Combine(_backupPath, "mysql", "backup_table.frm"), "tabledata");

            // create data files and directories (simulate existing DB)
            File.WriteAllText(Path.Combine(_dataPath, "ibdata1"), "important");
            File.WriteAllText(Path.Combine(_dataPath, "old.frm"), "oldfile");
            Directory.CreateDirectory(Path.Combine(_dataPath, "old_dir"));

            _dialog = new FakeDialogService();
        }

        [Fact]
        public void Repair_CopiesBackupAndPreservesIbdata1_ShowsSuccess()
        {
            var svc = new RepairService(_dialog);

            svc.Repair(_mysqlPath);

            // data_old must exist and contain previous data
            string dataOldPath = Path.Combine(_mysqlPath, "data_old");
            Assert.True(Directory.Exists(dataOldPath));
            Assert.True(File.Exists(Path.Combine(dataOldPath, "ibdata1")));
            Assert.True(File.Exists(Path.Combine(dataOldPath, "old.frm")));

            // ibdata1 must still exist in data folder
            Assert.True(File.Exists(Path.Combine(_dataPath, "ibdata1")));

            // file from backup must be copied into data folder
            Assert.True(File.Exists(Path.Combine(_dataPath, "schema.frm")));

            // directories from backup must be copied into data folder
            Assert.True(Directory.Exists(Path.Combine(_dataPath, "mysql")));
            Assert.True(File.Exists(Path.Combine(_dataPath, "mysql", "backup_table.frm")));

            // success dialog called
            Assert.True(_dialog.SuccessCalled);
            Assert.Null(_dialog.LastError);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_root))
                    Directory.Delete(_root, true);
            }
            catch
            {

            }
        }
    }
}