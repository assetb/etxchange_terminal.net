using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaOffice;

namespace DocumentFormation
{
    public static class AttachService
    {
        public static bool FillAttach(string templateFileName, Order order, string dealNumber = "", string customerBin = "", string brokerCustomerBin = "")
        {
            if (string.IsNullOrEmpty(templateFileName) || order == null || order.Auction == null || order.Auction.Lots == null || order.Auction.Lots.Count < 1 ||
                order.Auction.Lots[0].LotsExtended == null || order.Auction.Lots[0].LotsExtended.Count < 1) return false;

            var excel = new ExcelService(templateFileName);

            if (excel == null) return false;

            try {
                excel.SetCells(3, "C", order.Auction.Date != null ? order.Auction.Date.ToShortDateString() : "");
                excel.SetCells(4, "C", order.Number != null ? order.Number : "");
                excel.SetCells(5, "C", dealNumber != null ? dealNumber : "");
                excel.SetCells(7, "B", order.Initiator != null ? order.Initiator : "");
                excel.SetCells(8, "B", order.Organizer != null ? order.Organizer : "");
                excel.SetCells(7, "E", "БИН/ИИН " + customerBin != null ? customerBin : "");
                excel.SetCells(8, "E", "БИН/ИИН " + brokerCustomerBin != null ? brokerCustomerBin : "");

                var supplierOrder = order.Auction.SupplierOrders != null ? order.Auction.SupplierOrders.FirstOrDefault(s => s.status != null && s.status.Id != null && s.status.Id == 23) : new SupplierOrder();

                excel.SetCells(7, "H", supplierOrder != null ? supplierOrder.Name != null ? supplierOrder.Name : "" : "");
                excel.SetCells(8, "H", supplierOrder != null ? supplierOrder.BrokerName != null ? supplierOrder.BrokerName : "" : "");
                excel.SetCells(7, "L", "БИН/ИИН " + supplierOrder != null ? supplierOrder.BIN != null ? supplierOrder.BIN : "" : "");
                excel.SetCells(8, "L", "БИН/ИИН " + supplierOrder != null ? supplierOrder.BrokerBIN != null ? supplierOrder.BrokerBIN : "" : "");

                int rowCount = 10;
                int iCount = 1;

                foreach (var item in order.Auction.Lots[0].LotsExtended) {
                    if (iCount > 1) excel.InsertRow(rowCount + 1);

                    excel.SetCells(rowCount, "A", iCount);
                    excel.SetCells(rowCount, "B", item.name != null ? item.name : "");
                    excel.SetCells(rowCount, "C", item.unit != null ? item.unit : "");
                    excel.SetCells(rowCount, "D", item.quantity != null ? item.quantity : 0);
                    excel.SetCells(rowCount, "E", ""); // currency
                    excel.SetCells(rowCount, "F", item.price != null ? item.price : 0);
                    excel.SetCells(rowCount, "G", (item.price != null && item.quantity != null) ? item.price * item.quantity : 0);
                    excel.SetCells(rowCount, "H", item.endprice != null ? item.endprice : 0);
                    excel.SetCells(rowCount, "I", (item.endprice != null && item.quantity != null) ? item.endprice * item.quantity : 0);
                    excel.SetCells(rowCount, "J", (item.endsum != null && item.sum != null) ? item.endsum - item.sum : 0);
                    excel.SetCells(rowCount, "K", order.Auction.Lots[0].DeliveryTime != null ? order.Auction.Lots[0].DeliveryTime : "");
                    excel.SetCells(rowCount, "L", item.terms != null ? item.terms : "");
                    excel.SetCells(rowCount, "M", order.Auction.Lots[0].PaymentTerm != null ? order.Auction.Lots[0].PaymentTerm : "");

                    rowCount += 1;
                    iCount += 1;
                }

                excel.SetCells(rowCount, "G", order.Auction.Lots[0].LotsExtended.Sum(l => l.sum));
                excel.SetCells(rowCount, "I", order.Auction.Lots[0].LotsExtended.Sum(l => l.endsum));
                excel.SetCells(rowCount + 2, "C", order.Auction.Lots[0].LotsExtended.Sum(l => l.endsum));

                excel.CloseWorkbook(true);
                excel.CloseExcel();
            } catch {
                if (excel.IsWorkbookOpened()) excel.CloseWorkbook();
                if (excel.IsExcelOpened()) excel.CloseExcel();

                return false;
            }

            return true;
        }
    }
}