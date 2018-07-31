using System;
using AltaBO;
using AltaBO.specifics;
using DocumentFormation.office;
using AltaLog;

namespace DocumentFormation {
    public class OrderBP {

        public static Order GenerateOrder(CustomersEnum customerType, string customerOrderFile, string customerTreatyFile, Order order, string wherePath, string auctionNumber) {
            if(order == null) order = new Order();

            //Fill order with data from customer order files
            CustomerOrderService customerOrderService = null;

            if(customerType == CustomersEnum.Vostok) {
                if(order.Auction == null) order.Auction = new Auction();

                order.Auction.Lots = null;
                customerOrderService = new VostokOrderService_ref(customerOrderFile, customerTreatyFile);
            } else if(customerType == CustomersEnum.Inkay) {
                customerOrderService = new EnkayOrderService(new EnkayWordDocumentProvider(customerOrderFile));
                order.Warranty = "Настоящим подтверждаю действительность письма Гарантийного письма ТОО СП 'Инкай'   № 404 от «07» марта 2017 года и в случае его недействительности либо отказа Инициатора от его исполнения, принимаю на себя все указанные в Гарантийном письме обязательства.";
                order.Initiator = "ТОО 'Совмесное предприятие 'Инкай'(" + (auctionNumber.Length > 4 ? auctionNumber.Substring(auctionNumber.Length - 4) : auctionNumber) + ")  Юр.адрес: 161000, Южно-Казахстан обл., Сузакский район, пос.Тайконур, ул.Южная, 4  Факт.адрес: Южно-Казахстан обл., 160021, г.Шымкент, ул.Мадели кожа 1Г, б/ц ЭСКО, 5 этаж Реквизиты банка: Банк АО 'Ситибанк Казахстан', ИИК: KZ7383201T0200211006 БИК: CITIKZKA";
            } else if(customerType == CustomersEnum.KazMineralsService) {
                customerOrderService = new EnkayOrderService(new KazMinSerWordDocumentProvider(customerOrderFile));
                order.Warranty = "Настоящим письмом ТОО 'KAZ Minerals Service (КАЗ Минералс Сервис)', в соответствии с требованиями пунктов 35.4., 40.3. Правил торговли акционерного общества 'Товарная биржа 'Евразийская Торговая Система', утвержденных решением Совета Директоров АО 'Товарная биржа 'ЕТС' протокол №100 от 05.07.2013 г. (далее Правила), гарантирует заключение с потенциальными Победителями аукционов (далее Победители аукционов), проводимыми АО 'Товарная биржа 'ЕТС' на Секции торговли специализированными товарами по заявкам от ТОО 'KAZ Minerals Service' (КАЗ Минералз Сервис), (далее - Заявки), поданными с '05' января 2017 года по 31 декабря 2017 года, Договоров поставки товара на условиях, определенных в Заявках.\nНастоящая гарантия носит безотзывный характер.\nВ случаи нарушения нами настоящего обязательства, мы обязуемся по первому требованию Победителя аукциона уплатить Победителю аукциона штраф в размере, равном сумме биржевого обеспечения, ранее внесенного Победителем аукциона согласно требованиям Правил, для участия в аукционе по Заявке.";
                order.Initiator = "ТОО 'Kaz Minerals Service' (Каз Минералз Сервис) Юр.адрес: РК ВКО, г Усть-Каменогорск, проспект Победы 9/2 НП -34 Факт.адрес: РК ВКО, г Усть-Каменогорск, проспект Победы 9/2 НП -34 Реквизиты банка: Банк '', ИИК: KZ486010151000245849 БИК: ";
            }

            if(customerOrderService == null) return null;

            customerOrderService.UpdateOrder(order);

            if(customerType == CustomersEnum.Vostok) order.Initiator = order.Initiator.Insert(20, "(" + (auctionNumber.Length > 4 ? auctionNumber.Substring(auctionNumber.Length - 4) : auctionNumber) + ") ");

            //Make a order file for ets and copy it to provided destination
            var etsOrderService = new ETSOrderService();
            etsOrderService.MakeSaveCopyOrder(order, wherePath);

            //add attachments to order
            var attachService = customerType == CustomersEnum.Vostok ? new VostokAttachmentService(wherePath + "//Приложение к заявке №" + order.Auction.Number.Replace("/", "_") + ".docx") : (customerType == CustomersEnum.Inkay || customerType == CustomersEnum.KazMineralsService) ? (CustomerAttachmentService)(new EnkayAttachmentService(wherePath + "//Приложение к заявке №" + order.Auction.Number.Replace("/", "_") + ".docx")) : null;

            if(customerType == CustomersEnum.Vostok) {
                try {
                    customerOrderService.CopyQualificationsToBuffer();
                } catch(Exception ex) { AppJournal.Write("Order", "Formate order copy qualification error :" + ex.ToString(), true); }
                try {
                    attachService.PasteQualifications();
                } catch(Exception ex) { AppJournal.Write("Order", "Formate order paste qualification error :" + ex.ToString(), true); }

                int lCount = 2;

                foreach(var item in order.Auction.Lots) {
                    try {
                        if(order.Auction.Lots.Count == 1) customerOrderService.CopyTechSpecs();
                        else customerOrderService.CopyTechSpecs(lCount);
                    } catch(Exception ex) { AppJournal.Write("Order", "Formate order copy tech spec error :" + ex.ToString(), true); }

                    try {
                        attachService.PasteTechSpecs();
                    } catch(Exception ex) { AppJournal.Write("Order", "Formate order paste tech spec error :" + ex.ToString(), true); }

                    lCount++;
                }

                try {
                    customerOrderService.CopyAgreement();
                } catch(Exception ex) { AppJournal.Write("Order", "Formate order copy agreement error :" + ex.ToString(), true); }
                try {
                    attachService.PasteAgreements();
                } catch(Exception ex) { AppJournal.Write("Order", "Formate order paste agreement error :" + ex.ToString(), true); }
            } else if(customerType == CustomersEnum.Inkay || customerType == CustomersEnum.KazMineralsService) {
                customerOrderService.CopyQualificationsToBuffer();
                attachService.PasteAttachToETSOrder();
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
    }
}
