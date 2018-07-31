using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AltaBO;
using AltaOffice;

namespace DocumentFormation.office
{
    public class KazMinSerWordDocumentProvider:IEnkayDocumentProvider
    {
        private WordService service;


        public KazMinSerWordDocumentProvider(WordService service)
        {
            this.service = service;
        }


        public KazMinSerWordDocumentProvider(string orderDocumentFileName)
        {
            service = new WordService(orderDocumentFileName,false);
        }


        // Достаем номер аукциона
        public string GetAuctionNo()
        {
            var text = service.GetParagraph(1);

            var strLen = text.Length;
            var startIndex = text.IndexOf("№", StringComparison.Ordinal) + 1;

            return text.Substring(startIndex, (strLen - startIndex - 1));
        }


        // Достаем код участника
        public string GetMemberCode()
        {
            return service.GetCell(1, 1, 2);
        }


        // Достаем брокера
        public Broker GetBroker(Order order)
        {            
            order.Auction.Broker = new Broker(service.GetCell(1, 1, 2), "");
            order.Auction.Broker.Code = order.Auction.Broker.Code.ToUpper().Replace("К", "K").Replace("А", "A");
            
            switch (order.Auction.Broker.Code.ToLower().Substring(0,4))
            {
                case "altk":
                    order.Auction.Broker.Requisites = "ТОО «Альта и К», Андреев В.И., Директор Казахстан, 050064, г.Алматы, мкрн.Думан - 2, дом 18, кв. 55 тел.: +7(727) 390 - 43 - 02 e - mail: info @altaik.kz";
                    break;
                case "alta":
                    order.Auction.Broker.Requisites = "ТОО «Альтаир-Нур», Директор Кулик В.К., Адрес: Республика Казахстан, 050008, г.Алматы, ул.Карасай Батыра, уг.ул.Ауэзова, д. 183 / 19, оф. 409, тел.: +7(727) 259 - 72 - 32 e - mail: info @altairnur.kz";
                    break;
            }

            return order.Auction.Broker;
        }


        // Достаем процент биржевого обеспечения
        public string GetExchangeProvisionSize()
        {
            var text = service.GetCell(1, 9, 2);
            var reg = new Regex(@"[^\d,\.]*");
            text = reg.Replace(text, "").Replace(".", ",");
            return text;
        }


        // Достаем лоты
        public List<Lot> GetLots()
        {
            var lotList = service.GetCell(1, 8, 2);// tbl.Cell(8, 2).Range.Text;

            var lots = new List<Lot>();

            // 1 замена
            var pattern = "[-]";
            var replacement = "–";
            var rgx = new Regex(pattern);
            var result = rgx.Replace(lotList, replacement);

            // 2 замена
            pattern = "\\s+";
            replacement = " ";
            rgx = new Regex(pattern);
            result = rgx.Replace(result, replacement);

            // 3 замена
            pattern = "\\w[–]\\w";
            replacement = "-";
            rgx = new Regex(pattern);
            result = rgx.Replace(result, replacement);

            pattern = @"\b(Лот №\d+|Лот №\W?\d+)\b(.[^–]*)\W(([\d\s]*[,\.]{1}\d{2})|([\d\s]*))\s\(.";
            var iCount = 0;

            foreach (Match match in Regex.Matches(result, pattern))
            {
                iCount++;
                var lot = new Lot {
                    Number = iCount.ToString(),
                    Name = match.Groups[2].Value,
                    StartPrice = match.Groups[3].Value,
                    MinRequerments = "0"
                };
                lots.Add(lot);
            }

            return lots;
        }


        public void CopyQualificationsToBuffer()
        {
            DeleteUnnecessaries();
            Copy();
        }


        private void Copy()
        {
            service.CopyRange(0, service.GetEnd());
        }


        private void DeleteUnnecessaries()
        {
            service.DeleteParagraph(3);
            service.DeleteParagraph(2);
            service.DeleteParagraph(1);
            service.DeleteTableRows(1, new int[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 });
        }


        public void Close()
        {
            service.CloseWord(false);
        }

    }
}
