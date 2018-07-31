using AltaLog;
using System;
using System.IO;
using System.Windows.Forms;

namespace AltaTransport
{
    public static class Service
    {

        public static FileInfo GetFile(string title, string filter)
        {
            var dialog = new OpenFileDialog {
                Title = title,
                Filter = filter,
                FilterIndex = 0
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK && dialog.FileName.Length > 0) {
                var info = new FileInfo(dialog.FileName);

                if (info.Exists) return info;
            }

            return null;
        }


        public static FileInfo PutFile(string title, string filter, string fileName = "")
        {
            var dialog = new SaveFileDialog {
                Title = title,
                Filter = filter,
                FilterIndex = 0,
                FileName = fileName
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && dialog.FileName.Length > 0) {
                var info = new FileInfo(dialog.FileName);
                return info;
            }
            return null;
        }


        public static DirectoryInfo GetDirectory()
        {
            var dialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedPath.Length > 0) {
                var info = new DirectoryInfo(dialog.SelectedPath);
                if (info.Exists) return info;
            }
            return null;
        }


        public static bool CopyFile(string sourceFileName, string targetFileName, bool overwrite)
        {
            try {
                File.Copy(sourceFileName, targetFileName, overwrite);
            } catch (Exception ex) {
                AppJournal.Write("AltaArchive Service - File copy", "File copy error :" + ex.ToString(), true);
                return false;
            }

            return true;
        }


        public static void DeleteFile(string fileName)
        {
            try {
                File.Delete(fileName);
            } catch (Exception ex) { AppJournal.Write("AltaArchive Service - File delete", "File delete error :" + ex.ToString(), true); }
        }
    }
}
