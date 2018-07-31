using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerCabinetDemo.Models;

namespace CustomerCabinetDemo.Services
{
    public class DbStorage
    {
        #region Variables
        private static string[] GoodNames = CreateGoodNames();
        private static List<Good> GoodsList = CreateGoods();
        private static List<Lot> LotsList = CreateLots();
        private static List<Supplier> SuppliersList = CreateSuppliers();
        private static List<Auction> AuctionsList = CreateAuctions();
        #endregion

        #region Methods
        public static Good GetGood(string name) {
            return GoodsList.Where(g => g.name.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }

        public static List<Supplier> GetSuppliers()
        {
            return SuppliersList;
        }

        public static Supplier GetSupplier(string name)
        {
            return SuppliersList.Where(s => s.name == name).FirstOrDefault();
        }

        public static List<Supplier> GetSuppliersByGood(string key)
        {
            List<Supplier> suppliers = new List<Supplier>();

            foreach (var item in SuppliersList)
            {
                var supplier = item.goods.Where(g => g.name.ToLower().Contains(key.ToLower())).FirstOrDefault();

                if (supplier != null) suppliers.Add(item);
            }

            return suppliers;
        }

        public static IEnumerable<Auction> GetAuctions(bool status, bool isAll = false)
        {
            if (isAll) return AuctionsList;
            else return AuctionsList.Where(a => a.status == status);
        }

        public static Auction GetAuction(string auctionNumber)
        {
            return AuctionsList.Where(a => a.number == auctionNumber).FirstOrDefault();
        }

        public static AuctionDetails GetAuctionDetails(string auctionNumber)
        {
            AuctionDetails auctionDetails = new AuctionDetails();

            auctionDetails.auction = GetAuction(auctionNumber);

            if (!string.IsNullOrEmpty(auctionDetails.auction.winner))
            {
                auctionDetails.lot = LotsList.Where(l => l.name == auctionDetails.auction.lotName).FirstOrDefault();
                auctionDetails.suppliers = new List<Supplier>();

                int count = 0;

                foreach (var item in SuppliersList)
                {
                    var supplier = item.goods.Where(g => auctionDetails.lot != null && g.name == auctionDetails.lot.name).FirstOrDefault();

                    if (supplier != null && count == 0)
                    {
                        auctionDetails.suppliers.Add(item);
                        count++;
                    }
                    else if (supplier != null && count == 1)
                    {
                        auctionDetails.suppliers.Add(item);
                        count++;
                    }
                }

                if (auctionDetails.suppliers.Count < 2) auctionDetails.suppliers.Add(new Supplier());
            }
            else
            {
                auctionDetails.lot = new Lot();
                auctionDetails.suppliers = new List<Supplier>();

                auctionDetails.suppliers.Add(new Supplier());
                auctionDetails.suppliers.Add(new Supplier());
            }

            return auctionDetails;
        }

        public static void AddAuction(Auction auction)
        {
            AuctionsList.Add(auction);
        }

        private static List<Auction> CreateAuctions()
        {
            List<Auction> auctionsList = new List<Auction>();

            int count = GoodNames.Length;

            for (var i = 1; i < count; i++)
            {
                auctionsList.Add(new Auction()
                {
                    auctionDate = DateTime.Now.AddDays(i - count / 2),
                    number = i.ToString() + "/" + DateTime.Now.AddDays(i - count / 2).Month.ToString() + "-" + DateTime.Now.AddDays(i - count / 2).Year.ToString().Substring(2),
                    status = DateTime.Now.AddDays(i - count / 2) < DateTime.Now ? true : false,
                    lotName = LotsList[i - 1].name,
                    startPrice = LotsList[i - 1].startPrice,
                    winner = SuppliersList[i < 4 ? 0 : i > 35 ? 8 : i / 4].name,
                    orderDate = DateTime.Now.AddDays(i - count / 2 - 10)
                });
            }

            return auctionsList;
        }

        public static void CreateAuction(Auction auction)
        {
            AuctionsList.Add(auction);
        }

        private static string[] CreateGoodNames()
        {
            return new string[] { "Смазка","Канат","Фильтр","Термопары","Кольца","Изделия из ПВХ","Инструмент","Электроды","Сварочное оборудование","Осушитель",
            "Сетка","Вафельное полотно","Сторительные материалы","Электростанция","Дизельное топливо","Кронциркуль","Метизы","Противопожарное оборудование","Запасные части","Огнетушители",
            "Вкладыш","Поглотители","Химические средства","Электрооборудование","Кабели","Чистящие средства","Перчатки","Обувь",
            "Масло","Изоляция","Лакокрасочная продукция","Лента антискольжения","Трубы","Вал промежуточный","Плиты","Порошки",
            "Сода","Подшипники","Переналадки","Гипохлорид кальция","Аппараты"};
        }

        private static List<Good> CreateGoods()
        {
            List<Good> goodsList = new List<Good>();

            foreach (var item in GoodNames)
            {
                goodsList.Add(new Good() { name = item });
            }

            return goodsList;
        }

        private static List<Lot> CreateLots()
        {
            List<Lot> lotsList = new List<Lot>();

            int count = GoodNames.Length;

            for (var i = 1; i < count; i++)
            {
                lotsList.Add(new Lot()
                {
                    name = GoodNames[i - 1],
                    startPrice = GoodNames[i - 1].Length * 1000,
                    quantity = GoodNames[i - 1].Length,
                    paymentTerms = "По факту",
                    deliveryTerms = "Согласно договору",
                    deliveryPlace = "Склад заказчика",
                    deliveryTime = "В течении 30 дней"
                });
            }

            return lotsList;
        }

        private static List<Supplier> CreateSuppliers()
        {
            List<Supplier> suppliersList = new List<Supplier>();

            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'Welding Company'",
                bin = "070940026088",
                kbe = 17,
                country = "Казахстан",
                address = "г. Алматы, ул. Енисейская, дом № 26Б",
                telephones = "8 7273 17 88 89, 234 14 03, 251 11 53 (87212504024)",
                email = "sashamega@mail.ru",
                postcode = "050000",
                director = "Полторацкого А. А.",
                iik = "KZ058560000000429904",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk =3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'Oil VKO'",
                bin = "121040001133",
                kbe = 17,
                country = "Казахстан",
                address = "ВКО, г. Семей, ул. Усть-Каменогорская, дом № 2А",
                telephones = "8 7222 64 52 16, 8 777 650 25 85",
                email = "oilvko@bk.ru",
                postcode = "071413",
                director = "Таттыбаева Б. А.",
                iik = "KZ4994818KZT22030177",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk =3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'КРАВД'",
                bin = "120540020988",
                kbe = 17,
                country = "Казахстан",
                address = "ВКО, г. Усть-Каменогорск, ул. Михаэлиса, дом № 18-10",
                telephones = "8 7232 24 08 22, 8 7232 26 68 26",
                email = "seknin.vladimir@mail.ru",
                postcode = "070000",
                director = "Секнина В.Г.",
                iik = "KZ50914398409BC02787",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'Технология Комфорта'",
                bin = "050940007962",
                kbe = 17,
                country = "Казахстан",
                address = "071411, Республика Казахстан, Восточно-Казахстанская область, г. Семей., ул. Глинки, дом № 73г",
                telephones = "8 (7222) 354 795/ 520721/ 525838, 87071255975",
                email = "tehkomfort@mail.ru",
                postcode = "071411",
                director = "Мансурова Р. М.",
                iik = "KZ849650000032203174",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Общество с ограниченной ответственностью 'ГлавАвтоТранс'",
                bin = "2130025698",
                kbe = 21,
                country = "Россия",
                address = "Россия, Чувашская республика, г.Чебоксары,ул. Гузовского В.И., д.40",
                telephones = "(8352) 23 03 64, 23 03 65,23 03 67 Бухгалтерия(8352) 23 03 42",
                email = "mtogrupp@yandex.ru",
                postcode = "428000",
                director = "Жукова Н. В.",
                iik = "40702810300000012169",
                listConscientiousGos = 3,
                memberGos = 2,
                listConscientiousSamruk = 3,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'ЕвроЭлемент KZ'",
                bin = "120640002715",
                kbe = 17,
                country = "Казахстан",
                address = "ВКО, г. Усть-Каменогорск, ул. Авроры  181 А",
                telephones = "8 7232 53 19 07",
                email = "fl.kz.euroelement@yandex.kz",
                postcode = "070011",
                director = "Нарожнева О. А.",
                iik = "KZ959261001164071000",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'HARTEX'",
                bin = "130540001001",
                kbe = 17,
                country = "Казахстан",
                address = "РК, г. Усть-каменогорск, ул. Потанина 31-52",
                telephones = "8 705 600 81 58, 8 7232 53 61 21",
                email = "info.hartex@gmail.com",
                postcode = "072300",
                director = "Медведева М. С.",
                iik = "KZ78826F0KZTD2002673",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'Turkuaz Machinery' ('Туркуаз Машинери')'",
                bin = "031040002102",
                kbe = 17,
                country = "Казахстан",
                address = "г. Алматы, пр. Райымбека, 160а",
                telephones = "8 727 273 14 25, 273 15 68,  8 701 521 32 06 Татьяна",
                email = "t.rogovskaya@turkuazkz.com",
                postcode = "050016",
                director = "Четинелли М.К.",
                iik = "KZ75914398914BC04955",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });
            suppliersList.Add(new Supplier()
            {
                name = "Товарищество с ограниченной ответственностью 'BI-ART'",
                bin = "100440011367",
                kbe = 17,
                country = "Казахстан",
                address = "Карагандинская обл., г. Балхаш, ул. Караменде би, 29а",
                telephones = "8(71036) 4 43 33",
                email = "biart2008@gmail.com",
                postcode = "100300",
                director = "Горнинг Е. А.",
                iik = "KZ636010171000127335",
                listConscientiousGos = 3,
                listConscientiousSamruk = 3,
                memberGos = 1,
                memberSamruk = 3,
                listUnfairSamruk = 3
            });

            for (var i = 0; i < suppliersList.Count; i++)
            {
                suppliersList[i].goods = new List<Good>();

                for (var j = 0; j < suppliersList.Count; j++)
                {
                    suppliersList[i].goods.Add(GoodsList[j + (i * 4)]);
                }
            }
            return suppliersList;
        }
        #endregion
    }
}