using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaOffice;

namespace DocumentFormation {
    public class PostRegisterService {
        private static List<PostRegister> postRegister;
        private static ExcelService excel;
        private static string fileName;
        private static int iRow, iCount;
        private static Broker broker;

        public static void CreateDocument(List<PostRegister> postRegisterInfo, string fileNameItem, int mode, Broker brokerInfo) {
            postRegister = postRegisterInfo;
            fileName = fileNameItem;
            broker = brokerInfo;

            Filldocument(mode);
        }

        private static void Filldocument(int mode) {
            // Open excel
            excel = new ExcelService(fileName);

            switch(mode) {
                case 1: // Envelop
                    iRow = 1;
                    iCount = 1;

                    foreach(var item in postRegister) {
                        // Broker info
                        excel.SetCells(iRow, "B", broker.Name);
                        excel.SetCells(iRow + 1, "B", broker.Index);
                        excel.SetCells(iRow + 2, "B", broker.Address);
                        excel.SetCells(iRow + 3, "B", broker.Phones);

                        // Serial number
                        excel.SetCells(iRow, "D", iCount);

                        // Client info
                        excel.SetCells(iRow + 12, "D", item.name);
                        excel.SetCells(iRow + 13, "D", item.index);
                        excel.SetCells(iRow + 14, "D", item.address);
                        excel.SetCells(iRow + 15, "D", item.phones.Length > 44 ? item.phones.Substring(0, 44) : item.phones);

                        if(iCount != postRegister.Count) excel.CopyRange(1, "A", 16, "D", iRow + 17);

                        iRow += 18;
                        iCount++;
                    }
                    break;
                case 2: // Notification
                    iRow = 4;
                    iCount = 1;

                    foreach(var item in postRegister) {
                        // Broker info
                        excel.SetCells(iRow, "F", broker.Name);
                        excel.SetCells(iRow + 2, "F", broker.Address);

                        // Broker index
                        excel.SetCells(iRow + 4, "D", broker.Index.Substring(0, 1));
                        excel.SetCells(iRow + 4, "E", broker.Index.Substring(1, 1));
                        excel.SetCells(iRow + 4, "F", broker.Index.Substring(2, 1));
                        excel.SetCells(iRow + 4, "G", broker.Index.Substring(3, 1));
                        excel.SetCells(iRow + 4, "H", broker.Index.Substring(4, 1));
                        excel.SetCells(iRow + 4, "I", broker.Index.Substring(5, 1));

                        // Client info
                        excel.SetCells(iRow + 4, "AK", item.name);
                        excel.SetCells(iRow + 8, "AK", item.address);

                        // Client index
                        try {
                            excel.SetCells(iRow + 11, "AK", item.index.Substring(0, 1));
                            excel.SetCells(iRow + 11, "AL", item.index.Substring(1, 1));
                            excel.SetCells(iRow + 11, "AM", item.index.Substring(2, 1));
                            excel.SetCells(iRow + 11, "AN", item.index.Substring(3, 1));
                            excel.SetCells(iRow + 11, "AO", item.index.Substring(4, 1));
                            excel.SetCells(iRow + 11, "AP", item.index.Substring(5, 1));
                        } catch { }

                        if(iCount != postRegister.Count) excel.CopyRange(1, "A", 34, "BE", iRow + 34);

                        iRow += 38;
                        iCount++;
                    }
                    break;
                case 3: // Register
                    excel.SetCells(4, "B", broker.Name);

                    if (broker.Name.ToLower().Contains("корунд")) excel.SetCells(2, "E", "№0599/11.1-20-1550 от 27.07.2012г.");
                    else if (broker.Name.ToLower().Contains("альта")) excel.SetCells(2, "E", "№0599/11.1-14-698 от 01.04.2015г.");
                    else if (broker.Name.ToLower().Contains("алтын")) excel.SetCells(2, "E", "№0599/11.1-08-749 от 15.03.2016г.");

                    iRow = 9;

                    foreach(var item in postRegister) {
                        excel.SetCells(iRow, "A", (iRow - 8).ToString());
                        excel.SetCells(iRow, "B", item.name);
                        excel.SetCells(iRow, "C", item.index);
                        excel.SetCells(iRow, "D", item.address);
                        excel.SetCells(iRow, "E", item.phones);
                        excel.SetCells(iRow, "F", item.code);

                        excel.InsertRow(iRow + 1);
                        iRow++;
                    }
                    break;
            }

            // Close excel file
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }
    }
}
