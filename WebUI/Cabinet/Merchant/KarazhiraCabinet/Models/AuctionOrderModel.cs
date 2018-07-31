using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AltaBO;

namespace KarazhiraCabinet.Models {
    public class AuctionOrderModel {
        [Required(ErrorMessage = "Введите пожалуйста наименование продукта(лота)")]
        public string LotName { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста количество")]
        public string Count { get; set; }

        public int UnitId { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста стоимость за единицу")]
        public string Price { get; set; }

        public string Amount { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста шаг понижения")]
        public string Step { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста условия поставки")]
        public string DeliveryTerm { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста место поставки")]
        public string DeliveryPlace { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста время поставки")]
        public string DeliveryTime { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста условия оплаты")]
        public string Payment { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста биржевое обеспечение")]
        public string TradeWarranty { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста процент местного содержания")]
        public string Percent { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста минимальную сумму")]
        public string MinimalSum { get; set; }

        public string Comments { get; set; }

        public List<Unit> units { get; set; }
    }
}