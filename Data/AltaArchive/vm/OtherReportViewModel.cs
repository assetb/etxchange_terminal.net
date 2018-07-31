using AltaArchive.Services;
using AltaDock.vm;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaMySqlDB.model;
using AltaMySqlDB.service;
using AltaTransport;
using DocumentFormation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace AltaArchive.vm {
    public class OtherReportViewModel : BaseViewModel {
        #region Variables
        private IDataManager dataManager = new EntityContext();
        #endregion

        #region Methods
        public OtherReportViewModel() {
            DefaultParametrs();
        }

        private void DefaultParametrs() {
            StartDate = DateTime.Now.AddDays(-5);
            EndDate = DateTime.Now;
        }


        public ICommand TechSpecReportCmd { get { return new DelegateCommand(() => TechSpecReport(1)); } }
        public ICommand TechSpecReportAllCmd { get { return new DelegateCommand(() => TechSpecReport(2)); } }

        private void TechSpecReport(int type) {
            // Check for info for choosed data
            var lotExtended = DataBaseClient.ReadLotsExtended(StartDate, EndDate);

            if(lotExtended != null && lotExtended.Count > 0) {
                // Variables info
                string path = "";
                string templateFile = "";
                string templatePath = @"\\192.168.11.5\Archive\Templates\ETS\TechSpecReport.xlsx";

                // Get path to save template
                try {
                    path = Service.GetDirectory().FullName;
                } catch { path = ""; }

                if(!string.IsNullOrEmpty(path)) {
                    // Get template file
                    templateFile = path + "\\Отчет по тех. спец. с " + StartDate.ToShortDateString() + " по " + EndDate.ToShortDateString() + ".xlsx";

                    if(Service.CopyFile(templatePath, templateFile, true)) {
                        // Fill template file with info
                        try {
                            // Convert EF to BO
                            var techSpecReports = DataBaseClient.ReadTechSpecReport(1, StartDate, EndDate, new List<int>() { 4 }, type == 1 ? new List<int>() { 2 } : new List<int>() { 2, 3, 4 });

                            if(techSpecReports != null && techSpecReports.Count > 1) {

                                // Create report document
                                TechSpecReportService.CreateDocument(techSpecReports, templateFile);

                                // Open folder with file   
                                FolderExplorer.OpenFolder(path + "\\");
                            } else MessagesService.Show("Оповещение", "На эти даты записей нет");
                        } catch(Exception ex) { MessagesService.Show("Оповещение", "Произошла ошибка во время формирования отчета\n" + ex.ToString()); }
                    } else MessagesService.Show("Оповещение", "Произошла ошибка во время копирования шаблона");
                }
            } else MessagesService.Show("Оповещение", "Нет данных для формирования");
        }


        public ICommand FinalReportAllPLMTCmd { get { return new DelegateCommand(FinalReportAllPLMT); } }

        private void FinalReportAllPLMT() {
            // Variables info
            string path = "";
            string templateFile = "";
            string templatePath = @"\\192.168.11.5\Archive\Templates\ForAll\FinalReportPolyMetall.xlsx";

            // Get path to save template
            try {
                path = Service.GetDirectory().FullName;
            } catch { path = ""; }

            if(!string.IsNullOrEmpty(path)) {
                // Get template file
                templateFile = path + "\\Отчет по прошедшим с " + StartDate.ToShortDateString() + " по " + EndDate.ToShortDateString() + ".xlsx";

                if(Service.CopyFile(templatePath, templateFile, true)) {
                    // Get info from base
                    try {
                        var finalReportPlmtl = dataManager.GetFinalReportPlmtl(StartDate, EndDate);

                        if(finalReportPlmtl != null && finalReportPlmtl.Count > 0) {
                            // Create report document
                            TechSpecReportService.CreateDocument(finalReportPlmtl, templateFile);
                            // Open folder with file
                            FolderExplorer.OpenFolder(path + "\\");
                        } else {
                            Service.DeleteFile(templateFile);
                            MessagesService.Show("Оповещение", "На эти даты записей нет");
                        }
                    } catch(Exception ex) { MessagesService.Show("Оповещение", "Произошла ошибка во время формирования отчета\n" + ex.ToString()); }
                } else MessagesService.Show("Оповещение", "Произошла ошибка во время копирования шаблона");
            }
        }
        #endregion

        #region Bindings
        private DateTime _startDate;
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; RaisePropertyChangedEvent("StartDate"); }
        }

        private DateTime _endDate;
        public DateTime EndDate {
            get { return _endDate; }
            set { _endDate = value; RaisePropertyChangedEvent("EndDate"); }
        }
        #endregion
    }
}
