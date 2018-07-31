using System;
using System.Collections.ObjectModel;
using System.Linq;
using AltaBO;

namespace DocumentFormation
{
    /// <summary>
    /// Class to operate with Enkay customer incoming file to get Order.
    /// It uses windows buffer to copy attachments to output files.
    /// </summary>
    public class EnkayOrderService : CustomerOrderService
    {
        private IEnkayDocumentProvider _provider;

        public EnkayOrderService(IEnkayDocumentProvider p)
        {            
            _provider = p;
        }


        private const string WarrantyLetter = "Настоящим подтверждаю действительность письма Гарантийного письма ТОО СП 'Инкай'   № 404 от «07» марта 2017 года и в случае его недействительности либо отказа Инициатора от его исполнения, принимаю на себя все указанные в Гарантийном письме обязательства.";
        private const string FullNameOfInitiator = "ТОО 'Совмесное предприятие 'Инкай'  Юр.адрес: 161000, Южно-Казахстан обл., Сузакский район, пос.Тайконур, ул.Южная, 4  Факт.адрес: Южно-Казахстан обл., 160021, г.Шымкент, ул.Мадели кожа 1Г, б/ц ЭСКО, 5 этаж Реквизиты банка: Банк АО 'Ситибанк Казахстан', ИИК: KZ7383201T0200211006 БИК: CITIKZKA";

        public override bool CopyAgreement()
        {
            throw new NotImplementedException();
        }

        public override bool CopyQualificationsToBuffer()
        {
            _provider.CopyQualificationsToBuffer();
            return true;
        }

        public override bool CopyTechSpecs()
        {
            throw new NotImplementedException();
        }


        public override Order UpdateOrder(Order order)
        {
            // Поиск и присвоение номера аукциона            
            order.Title = _provider.GetAuctionNo() + " от " + (order.Date.Hour > 12 ? order.Date.AddDays(1) : order.Date).ToShortDateString();

            // Выборка брокера по коду
            order.Auction.Broker = _provider.GetBroker(order);

            // Поиск и присвоение процента биржевого обеспечения
            order.Auction.ExchangeProvisionSize = _provider.GetExchangeProvisionSize();

            // Поиск и формирование списка лотов
            order.Auction.Lots = new ObservableCollection<Lot>(_provider.GetLots()/*.Reverse<Lot>()*/);

            // Гарантийное письмо
            //order.Warranty = WarrantyLetter;

            // Полное наименование инициатора
            //order.Initiator = FullNameOfInitiator;

            return order;
        }


        public override void Close()
        {
            _provider.Close();
            _provider = null;
        }

        public override bool CopyTechSpecs(int sheetIndex) {
            throw new NotImplementedException();
        }
    }
}
