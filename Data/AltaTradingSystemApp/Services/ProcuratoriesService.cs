using AltaArchiveApp;
using AltaBO;
using AltaBO.specifics;
using AltaMySqlDB.service;
using DocumentFormation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaTradingSystemApp.Services
{
    public static class ProcuratoriesService
    {

        public static bool AppendProcuratory(IDataManager dataManager, ArchiveManager archiveManager, List<Procuratory> procuratories, string pathToTemplate, string pathToScan) {
            var procuratory = procuratories.First();
            if (procuratory == null)
            {
                return false;
            }

            var auction = dataManager.GetAuction((int)procuratory.auctionId);
            if (auction == null)
            {
                return false;
            }

            var supplierOrder = dataManager.GetSupplierOrder(auction.Id, procuratory.SupplierId);
            if (supplierOrder == null)
            {
                return false;
            }

            var supplierJournal = dataManager.GetSupplierJournal(supplierOrder.brokerid, procuratory.SupplierId);
            if (supplierJournal == null)
            {
                return false;
            }

            var filelistId = archiveManager.CreateFilesList("Списко поручений");

            var scanId = archiveManager.PutDocument(pathToScan, new DocumentRequisite()
            {
                date = auction.Date,
                market = (MarketPlaceEnum)auction.SiteId,
                number = auction.Number,
                fileName = Path.GetFileName(pathToScan),
                section = DocumentSectionEnum.Auction,
                type = DocumentTypeEnum.Procuratory
            }, filelistId);
            if (scanId < 1)
            {
                return false;
            }


            var templateId = archiveManager.PutDocument(pathToTemplate, new DocumentRequisite()
            {
                date = auction.Date,
                market = (MarketPlaceEnum)auction.SiteId,
                number = auction.Number,
                fileName = Path.GetFileName(pathToScan),
                section = DocumentSectionEnum.Auction,
                type = DocumentTypeEnum.ProcuratorySource
            }, filelistId);
            if (templateId < 1)
            {
                return false;
            }

            procuratories.ForEach(p => p.fileListId = filelistId);

            return dataManager.AddProcuratories(auction.Id, procuratories);
        }


        public static bool GenerateProcuratoryFile(IDataManager dataManager, ArchiveManager archiveManager, List<Procuratory> procuratories, string saveTo, bool autoCounting = false) {

            var procuratory = procuratories.First();
            if (procuratory == null) {
                return false;
            }

            var auction = dataManager.GetAuction((int)procuratory.auctionId);
            if (auction == null) {
                return false;
            }

            var supplierOrder = dataManager.GetSupplierOrder(auction.Id, procuratory.SupplierId);
            if (supplierOrder == null)
            {
                return false;
            }

            var supplierJournal = dataManager.GetSupplierJournal(supplierOrder.brokerid, procuratory.SupplierId);
            if (supplierJournal == null)
            {
                return false;
            }

            var templateRequisite = archiveManager.GetTemplateRequisite((MarketPlaceEnum)auction.SiteId, DocumentTemplateEnum.Procuratory);
            if (templateRequisite == null)
            {
                return false;
            }

            archiveManager.GetDocument(templateRequisite, saveTo);

            supplierOrder.Code = supplierJournal.code;
            var order = new Order();
            order.Auction = auction;
            order.Auction.SupplierOrders.Clear();
            order.Auction.SupplierOrders.Add(supplierOrder);
            order.Auction.Procuratories.Clear();
            procuratories.ForEach(p => order.Auction.Procuratories.Add(p));

            DocumentFormation.ProcuratoriesService.FormateProcuratory(saveTo, order, autoCounting: autoCounting);

            return true;
        }
    }
}
