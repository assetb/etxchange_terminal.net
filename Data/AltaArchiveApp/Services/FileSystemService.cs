using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltaArchiveApp.Services
{
    public static class FileSystemService
    {
        public static string GetFile(string title = "Выберите файл")
        {
            OpenFileDialog openFD = new OpenFileDialog();
            openFD.Title = title;

            if (openFD.ShowDialog() == DialogResult.OK) return openFD.FileName;
            
            return null;
        }


        public static bool CopyFile(string oldPath, string newPath)
        {
            try
            {
                File.Copy(oldPath, newPath, true);
                return true;
            }
            catch { return false; }
        }


        public static string SaveFile(string filter, string title = "Укажите файл для сохранения")
        {
            SaveFileDialog saveFD = new SaveFileDialog();
            saveFD.Title = title;
            saveFD.Filter = filter;

            if (saveFD.ShowDialog() == DialogResult.OK)
            {
                //File.Create(saveFD.FileName).Dispose();
                return saveFD.FileName;
            }

            return null;
        }
    }
}
