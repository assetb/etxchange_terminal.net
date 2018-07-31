using altaik.baseapp.vm;
using System.Collections.ObjectModel;

namespace AltaBO
{
    public class Lot : BaseViewModel
    {
        public int Id { get; set; }

        public int auctionId { get; set; }

        private string number;
        public string Number { get { return number; } set { number = value; RaisePropertyChangedEvent("Number"); } }

        private string name;
        public string Name { get { return name; } set { name = value; RaisePropertyChangedEvent("Name"); } }

        private string unit;
        public string Unit { get { return unit; } set { unit = value; RaisePropertyChangedEvent("Unit"); } }

        public int UnitId { get; set; }

        private string codeTRFEA;
        public string CodeTRFEA { get { return codeTRFEA; } set { codeTRFEA = value; RaisePropertyChangedEvent("CodeTRFEA"); } }

        private decimal qunatity;
        public decimal Quantity { get { return qunatity; } set { qunatity = value; RaisePropertyChangedEvent("Quantity"); } }

        private decimal price;
        public decimal Price { get { return price; } set { price = value; RaisePropertyChangedEvent("Price"); } }

        private decimal sum;
        public decimal Sum { get { return sum; } set { sum = value; RaisePropertyChangedEvent("Sum"); } }

        private string paymentTerm;
        public string PaymentTerm { get { return paymentTerm; } set { paymentTerm = value; RaisePropertyChangedEvent("PaymentTerm"); } }

        private string deliveryTime;
        public string DeliveryTime { get { return deliveryTime; } set { deliveryTime = value; RaisePropertyChangedEvent("DeliveryTime"); } }

        private string deliveryPlace;
        public string DeliveryPlace { get { return deliveryPlace; } set { deliveryPlace = value; RaisePropertyChangedEvent("DeliveryPlace"); } }

        private int dks;
        public int Dks { get { return dks; } set { dks = value; RaisePropertyChangedEvent("Dks"); } }

        private string contractNumber;
        public string ContractNumber {
            get { return contractNumber; }
            set { contractNumber = value; RaisePropertyChangedEvent("ContractNumber"); }
        }

        private decimal step;
        public decimal Step { get { return step; } set { step = value; RaisePropertyChangedEvent("Step"); } }

        private decimal warranty;
        public decimal Warranty { get { return warranty; } set { warranty = value; RaisePropertyChangedEvent("Warranty"); } }

        private decimal localContent;
        public decimal LocalContent { get { return localContent; } set { localContent = value; RaisePropertyChangedEvent("LocalContent"); } }

        private string minRequerments;
        public string MinRequerments { get { return minRequerments; } set { minRequerments = value; RaisePropertyChangedEvent("MinRequerments"); } }

        private string startPrice;
        public string StartPrice { get { return startPrice; } set { startPrice = value; RaisePropertyChangedEvent("StartPrice"); } }

        public int fileListId { get; set; }

        private ObservableCollection<LotsExtended> lotsextended;
        public ObservableCollection<LotsExtended> LotsExtended {
            get { if (lotsextended == null) lotsextended = new ObservableCollection<LotsExtended>(); return lotsextended; }
            set { if (value != lotsextended) { lotsextended = value; RaisePropertyChangedEvent("LotsExtended"); } }
        }

    }
}
