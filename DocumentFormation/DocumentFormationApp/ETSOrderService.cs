using AltaBO;
using AltaBO.specifics;
using AltaOffice;
using AltaTransport;
using DocumentFormation.office;
using System;
using System.Diagnostics;
using System.IO;

namespace DocumentFormation
{
    public class ETSOrderService {
        private const string WARRANTY_LETTER = "[Гарантийное письмо]";
        private const string TEMPLATE_FILE_NAME = "Order.xlsx";

        private string where = "../data/";
        /*
                private string fName = "";
        */
        private ExcelService service;
        private Order order { get; set; }


        public static Order GenerateOrder(CustomersEnum customerType, string customerOrderFile, string customerTreatyFile, Order order, string wherePath) {
            if(order == null) order = new Order();

            //fill order with data from customer order files
            CustomerOrderService customerOrderService = null;
            if(customerType == CustomersEnum.Vostok) {
                if(order.Auction == null) order.Auction = new Auction();
                order.Auction.Lots = null;
                customerOrderService = new VostokOrderService_ref(customerOrderFile, customerTreatyFile);
            }
            if(customerType == CustomersEnum.Inkay) customerOrderService = new EnkayOrderService(new EnkayWordDocumentProvider(customerOrderFile));
            if(customerOrderService == null) return null;
            customerOrderService.UpdateOrder(order);

            //make a order file for ets and copy it to provided destination
            var etsOrderService = new ETSOrderService();
            etsOrderService.MakeSaveCopyOrder(order, wherePath);

            //add attachments to order
            var attachService = customerType == CustomersEnum.Vostok ? new VostokAttachmentService(wherePath + "//Приложение к заявке №" + GetOrderNo(order) + ".docx") : customerType == CustomersEnum.Inkay ? (CustomerAttachmentService)(new EnkayAttachmentService(wherePath + "//Приложение.docx")) : null;
            if(customerType == CustomersEnum.Vostok) {
                attachService = new VostokAttachmentService(wherePath + "//Приложение к заявке №" + GetOrderNo(order) + ".docx");
                customerOrderService.CopyQualificationsToBuffer();
                attachService.PasteQualifications();
                customerOrderService.CopyTechSpecs();
                attachService.PasteTechSpecs();
                customerOrderService.CopyAgreement();
                attachService.PasteAgreements();
            }
            if(customerType == CustomersEnum.Inkay) {
                attachService = new EnkayAttachmentService(wherePath + "//Приложение.docx");
                attachService.PasteAllAttach();
            }

            attachService?.Close();
            etsOrderService.Close();
            if(customerOrderFile != null) customerOrderService.Close();

            return order;
        }


        private static string GetOrderNo(Order order) {
            var fName = order.Title.Substring(0, order.Title.IndexOf("от", StringComparison.Ordinal) - 1);

            if(fName.Length > 4) fName = fName.Substring(fName.Length - 4, 4);

            return fName;
        }


        public bool MakeSaveCopyOrder(Order orderImp, string whereImp) {
            order = orderImp;

            if(order == null || string.IsNullOrEmpty(whereImp)) return false;
            order = orderImp;
            where = whereImp;

            service = new ExcelService(where + "\\Заявка №" + orderImp.Auction.Number.Replace("/", "_") + ".xlsx");

            SetAuctionNo();
            SetAuctionDate();
            SetInitiator();
            SetMemberCode();
            SetExchangeProvisionSize();
            SetOrderDeadline();
            SetApplicantsDeadline();
            SetExchangeProvisionDeadline();
            SetBroker();
            SetOrderDate();
            SetLots();
            SetWarrantyLetterInfo();

            Close();

            return true;
        }


        private void Copy() {
            if(!Directory.Exists(where)) return;

            try {
                System.IO.File.Copy(FileArchiveTransport.GetTemplatesPath() + "\\" + TEMPLATE_FILE_NAME, where + "\\Заявка.xlsx", true);
            } catch(Exception ex) { Debug.Write("Внимание: Problem with copying Order file " + ex.Message); }
        }

        #region Setter Functions
        // Номер заявки
        private void SetAuctionNo() {
            service.SetCells(7, "A", "Заявка на проведение биржевого аукциона №" + order.Auction.Number + " от " + order.Date.ToShortDateString());
        }


        // Дата аукциона
        private void SetAuctionDate() {
            service.SetCells(9, "A", order.Auction.Date.ToShortDateString());
        }


        // Полное наименование инициатора
        private void SetInitiator() {
            service.SetCells(9, "F", order.Initiator);
        }


        // Код участника торгов
        private void SetMemberCode() {
            if(order.Auction.Broker.Code.Length > 4) order.Auction.Broker.Code = order.Auction.Broker.Code.Substring(0, 4);

            service.SetCells(9, "G", order.Auction.Broker.Code);
        }


        // Размер биржевого обеспечения
        private void SetExchangeProvisionSize() {
            service.SetCells(9, "H", !string.Equals(order.Auction.ExchangeProvisionSize, null, StringComparison.Ordinal) ? order.Auction.ExchangeProvisionSize : "");
        }


        // Срок подачи заявок на участие в аукционе
        private void SetOrderDeadline() {
            service.SetCells(9, "L", order.Deadline.ToShortDateString() + " 16:00");
        }


        // Срок подачи бирже списка претендентов, допущенных до участия в аукционе
        private void SetApplicantsDeadline() {
            try {
                service.SetCells(9, "M", order.Auction.ApplicantsDeadline.ToShortDateString());
            } catch(Exception) { }
        }


        // Срок внесения биржевого обеспечения
        private void SetExchangeProvisionDeadline() {
            service.SetCells(9, "N", order.Auction.ExchangeProvisionDeadline.ToShortDateString());
        }


        // Уполномоченное лицо инициатора
        private void SetBroker() {
            service.SetCells(9, "O", order.Auction.Broker.Requisites);
        }


        // Дата подачи заявки
        private void SetOrderDate() {
            service.SetCells(9, "P", order.Date.ToShortDateString());
        }


        // Информация о лоте
        private void SetLots()
        {
            const int INDEX = 13;
            var row = INDEX;
            int rowCount = order.Auction.Lots.Count;

            if(rowCount > 1) {
                for(var iCount = 1; iCount < rowCount; iCount++) {
                    row = service.InsertRow(INDEX);
                }

                row = INDEX;
            }

            foreach(var lot in order.Auction.Lots) {
                try {
                    service.SetCells(row, "A", lot.Number);
                    service.SetCells(row, "B", lot.Name, 1);
                    service.SetCellWrapText(row, "B", true);

                    var sPrice = lot.StartPrice == null ? "0" : lot.StartPrice;

                    if(!sPrice.Contains(",") && !sPrice.Contains(".")) sPrice += ",00";

                    service.SetCells(row, "C", sPrice, 1);
                    service.SetCells(row, "D", lot.MinRequerments, 1);
                } catch { }

                row++;
            }
        }


        // Информация о гарантийном письме
        private void SetWarrantyLetterInfo() {
            try {
                var row = service.FindRow(WARRANTY_LETTER);
                service.SetCells(row, "B", order.Warranty, "@");
            } catch(Exception) { }
        }
        #endregion

        // Закрываем и сохраняем изменения в Excel
        public void Close() {
            service.CloseWorkbook(true);
            service.CloseExcel();
        }
    }
}
