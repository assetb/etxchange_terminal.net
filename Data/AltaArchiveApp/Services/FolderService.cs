using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AltaArchiveApp.Services {
    public static class FolderService {
        private static string iPPath = @"\\192.168.11.5";
        private static FolderBrowserDialog folderBD = new FolderBrowserDialog();


        public static string ChooseFolderPath(string title = "Выберите путь") {
            folderBD.Description = title;
            folderBD.Tag = folderBD.Description;

            if(folderBD.ShowDialog() == DialogResult.OK) {
                return folderBD.SelectedPath;
            }

            return null;
        }


        public static string GetFolderPath(int presentId, string folderName, List<Node> parentNodes) {
            string address = "";

            switch(presentId) {
                // Archive
                case 1:
                    address = iPPath + @"\Archive\Archive";

                    foreach(var item in parentNodes) {
                        address += @"\" + item.Name;
                        if(!CheckFolderExist(address)) CreateFolder(address);
                    }
                    break;
                // Exchange
                case 2: break;
            }

            address += @"\" + folderName + @"\";

            if(!CheckFolderExist(address)) CreateFolder(address);

            return address;
        }


        public static string GetFolderPath(int presentId, string folderName, string parentNodes) {
            string address = "";

            switch(presentId) {
                // Archive
                case 1:
                    address = iPPath + @"\Archive\Archive";
                    string[] path = parentNodes.Split('\\');

                    foreach(var item in path) {
                        address += @"\" + item;
                        if(!CheckFolderExist(address)) CreateFolder(address);
                    }

                    break;
                // Exchange
                case 2: break;
            }

            return address + @"\";
        }


        public static void CreateFolder(int presentId, string folderName, List<Node> parentNodes) {
            string address = "";

            switch(presentId) {
                // Archive
                case 1:
                    address = iPPath + @"\Archive\Archive";

                    foreach(var item in parentNodes) {
                        address += @"\" + item.Name;
                        if(!CheckFolderExist(address)) CreateFolder(address);
                    }
                    break;
                // Exchange
                case 2: break;
            }

            address += @"\" + folderName;
            if(!CheckFolderExist(address)) CreateFolder(address);
        }


        public static void DeleteFolder(int presentId, string folderName, List<Node> parentNodes) {
            string address = "";

            switch(presentId) {
                // Archive
                case 1:
                    address = iPPath + @"\Archive\Archive";

                    foreach(var item in parentNodes) {
                        address += @"\" + item.Name;
                    }
                    break;
                // Exchange
                case 2: break;
            }

            address += @"\" + folderName;
            if(CheckFolderExist(address)) DeleteFolder(address);
        }


        public static bool RenameFolder(int presentId, string folderOldName, string folderNewName, List<Node> parentNodes) {
            string address = "";

            switch(presentId) {
                // Archive
                case 1:
                    address = iPPath + @"\Archive\Archive";

                    foreach(var item in parentNodes) {
                        address += @"\" + item.Name;
                    }
                    break;
                // Exchange
                case 2: break;
            }

            string oldAddress = address + @"\" + folderOldName;
            string newAddress = address + @"\" + folderNewName;

            if(CheckFolderExist(oldAddress)) {
                try {
                    RenameFolder(oldAddress, newAddress);
                    return true;
                } catch { return false; }
            }

            return false;
        }


        private static bool CheckFolderExist(string url) {
            return Directory.Exists(url);
        }


        private static void CreateFolder(string url) {
            Directory.CreateDirectory(url);
        }


        private static void DeleteFolder(string url) {
            Directory.Delete(url);
        }


        private static void RenameFolder(string oldUrl, string newUrl) {
            Directory.Move(oldUrl, newUrl);
        }


        public static void OpenFolder(string path)
        {
            Process.Start("explorer", path);
        }
    }
}
