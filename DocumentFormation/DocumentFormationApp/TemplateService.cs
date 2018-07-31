using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO.specifics;
using AltaArchiveApp;
using System.IO;

namespace DocumentFormation
{
    public class TemplateService
    {
        #region Variables
        private ArchiveManager archiveManager;
        #endregion

        #region Methods
        public TemplateService(ArchiveManager archiveManager)
        {
            this.archiveManager = archiveManager;
        }

        public TemplateService() : this(new ArchiveManager()) { }

        public string GetTemplatePath(DocumentTemplateEnum docTemplateType, MarketPlaceEnum marketType, string extension)
        {
            return archiveManager.GetTemplatePath(archiveManager.GetPath(new AltaBO.DocumentRequisite()
            {
                section = DocumentSectionEnum.Template,
                market = marketType,
                fileName = string.Format("{0}.{1}", docTemplateType, extension)
            }));
        }

        public Stream GetTemplateStream(DocumentTemplateEnum docTemplateType, MarketPlaceEnum marketType, string extension)
        {
            return archiveManager.GetTemplateStream(archiveManager.GetPath(new AltaBO.DocumentRequisite()
            {
                section = DocumentSectionEnum.Template,
                market = marketType,
                fileName = string.Format("{0}.{1}", docTemplateType, extension),
            }));
        }
        #endregion
    }
}
