using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO.specifics;
using System.IO;

namespace AltaArchive.Services {
    public class FolderExplorer {
        private static string AuctionFolder = @"\\192.168.11.5\Archive\Auctions\";
        private static string CompanyFolder = @"\\192.168.11.5\Archive\Companies\";


        public static void OpenAuctionFolder(MarketPlaceEnum marketPlace, string auctionDate, string auctionNumber) {
            if(marketPlace == MarketPlaceEnum.UTB || marketPlace == MarketPlaceEnum.KazETS) Process.Start("explorer", AuctionFolder + marketPlace + "\\" + auctionDate + "\\" + auctionNumber.Replace("/", "_"));
            else {
                string oldPath = AuctionFolder + marketPlace + "\\" + auctionDate + "\\" + (auctionNumber.Length > 4 ? auctionNumber.Substring(auctionNumber.Length - 4) : auctionNumber).Replace("/", "_");
                string newPath = AuctionFolder + marketPlace + "\\" + auctionDate + "\\" + auctionNumber.Replace("/", "_");
                string path = Directory.Exists(oldPath) ? oldPath : newPath;

                Process.Start("explorer", path);
            }
        }


        public static string GetAuctionPath(MarketPlaceEnum marketPlace, string auctionDate, string auctionNumber) {
            string oldPath = AuctionFolder + marketPlace + "\\" + auctionDate + "\\" + ((marketPlace == MarketPlaceEnum.UTB || marketPlace == MarketPlaceEnum.KazETS) ? auctionNumber : auctionNumber.Length > 4 ? auctionNumber.Substring(auctionNumber.Length - 4) : auctionNumber).Replace("/", "_");
            string newPath = AuctionFolder + marketPlace + "\\" + auctionDate + "\\" + auctionNumber.Replace("/", "_");
            string path = Directory.Exists(oldPath) ? oldPath : newPath;

            return path + "\\";
        }


        public static bool CheckInvoice(MarketPlaceEnum marketPlace, string auctionDate, string auctionNumber, string pattern) {
            string[] files = Directory.GetFiles(GetAuctionPath(marketPlace, auctionDate, auctionNumber), pattern);

            if(files.Length > 0) {
                if(File.Exists(files[0])) return true;
                else return false;
            }

            return false;
        }


        public static string[] CheckFiles(MarketPlaceEnum marketPlace, string auctionDate, string auctionNumber, string pattern) {
            string[] result;

            try {
                result = Directory.GetFiles(GetAuctionPath(marketPlace, auctionDate, auctionNumber.Replace("/","_")), pattern);
            } catch {
                result = null;
            }

            return result;
        }


        public static void CreateCompanyFolder(string companyBin) {
            if(!Directory.Exists(CompanyFolder + "\\" + companyBin)) Directory.CreateDirectory(CompanyFolder + "\\" + companyBin);
        }


        public static string GetCompanyPath(string bin) {
            CreateCompanyFolder(bin);

            return CompanyFolder + bin + "\\";
        }


        public static void OpenCompanyFolder(string bin) {
            CreateCompanyFolder(bin);
            Process.Start("explorer", CompanyFolder + bin + "\\");
        }


        public static void OpenFolder(string path) {
            Process.Start("explorer", path);
        }


        public static bool CopyFile(string oldFile, string newFile, bool isOverwrite = true) {
            if(string.IsNullOrEmpty(oldFile) || string.IsNullOrEmpty(newFile)) return false;
            if(!File.Exists(oldFile)) return false;

            try {
                File.Copy(oldFile, newFile, isOverwrite);
                return true;
            } catch { return false; }
        }
    }
}
