using AltaBO;
using AltaBO.specifics;
using DocumentFormation;
using ServerApp.Models;
using ServerApp.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/report")]
    public class ReportController : BaseApiController
    {
        private const string REPORT_OF_THE_PERIOD = "Report_for_the_period";

        [HttpGet, Route("tech_spec"), Authorize(Roles = "customer")]
        public Message<List<TechSpecReportBO>> GetReport(DateTime startDate, DateTime endDate, string dateFilterType, int siteId = 0, int statusId = 0, short sortMode = 0, string sortColumnName = null)
        {
            if (CurrentUser.PersonId != 17)
            {
                List<int> sites = new List<int>();
                List<int> statuses = new List<int>();

                if (siteId > 0)
                {
                    sites.Add(siteId);
                }

                switch (statusId)
                {
                    case (1):
                        {
                            statuses.Add(2);
                            //statuses.Add(3);
                        }
                        break;
                    case (2):
                        {
                            statuses.Add(1);
                            statuses.Add(4);
                            statuses.Add(5);
                        }
                        break;
                }
                var message = new Message<List<TechSpecReportBO>>();
                try
                {
                    List<TechSpecReportBO> reports = null;
                    switch (dateFilterType)
                    {
                        case ("auctionDate"):
                            {
                                reports = DataManager.GetTechSpecReport(CurrentUser.CustomerId, startDate, endDate, sites, statuses, sortMode: sortMode, sortColumnName: sortColumnName);
                            }
                            break;
                        case ("orderDate"):
                            {
                                reports = DataManager.GetTechSpecReportByOrderDate(CurrentUser.CustomerId, startDate, endDate, sites, statuses, sortMode: sortMode, sortColumnName: sortColumnName);
                            }
                            break;
                    }
                    message.code = 200;

                    message.data = reports;
                }
                catch (Exception ex)
                {
                    message.code = 600;
                    message.description = ex.Message;
                }
                return message;
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }

        [HttpGet, Route("tech_spec/generate"), Authorize(Roles = "customer")]
        public HttpResponseMessage GenerateReport(DateTime startDate, DateTime endDate, string dateFilterType, int siteId = 0, int statusId = 0)
        {
            if (CurrentUser.PersonId != 17)
            {
                List<int> sites = new List<int>();
                List<int> statuses = new List<int>();

                if (siteId > 0)
                {
                    sites.Add(siteId);
                }

                switch (statusId)
                {
                    case (1):
                        {
                            statuses.Add(2);
                            //statuses.Add(3);
                        }
                        break;
                    case (2):
                        {
                            statuses.Add(1);
                            statuses.Add(4);
                            statuses.Add(5);
                        }
                        break;
                }
                List<TechSpecReportBO> reports = null;

                switch (dateFilterType)
                {
                    case ("auctionDate"):
                        {
                            reports = DataManager.GetTechSpecReport(CurrentUser.CustomerId, startDate, endDate, sites, statuses);
                        }
                        break;
                    case ("orderDate"):
                        {

                            reports = DataManager.GetTechSpecReportByOrderDate(CurrentUser.CustomerId, startDate, endDate, sites, statuses);
                        }
                        break;
                }

                if (reports == null || reports.Count == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                var templateRequisite = ArchiveManager.GetTemplateRequisite(MarketPlaceEnum.ETS, DocumentTemplateEnum.TechSpecReport);
                if (templateRequisite == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotImplemented);
                }

                var localPath = ArchiveManager.LoadTemplateToLocalStorage(templateRequisite);
                TechSpecReportService.CreateDocument(reports, localPath);
                var fileName = string.Format("{0}_from_{1:dd-MM-yyyy}_to_{2:dd-MM-yyyy}.{3}", REPORT_OF_THE_PERIOD, startDate, endDate, templateRequisite.extension);
                var responceFile = HttpResponceFile.Create(fileName, localPath);

                if (responceFile == null)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                return responceFile;
            }
            else
            {
                HttpContext.Current.Response.AddHeader("x-is-active", false.ToString());
                return null;
            }
        }
    }
}
