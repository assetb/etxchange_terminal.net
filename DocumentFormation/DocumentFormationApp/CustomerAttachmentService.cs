using System;
using System.IO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public abstract class CustomerAttachmentService
    {
        protected string TEMPLATE_FILE_NAME = "OrderAttach.docx";
        public WordService service;
        private string outName;


        public CustomerAttachmentService(string toFileName)
        {
            //SaveAs(toFileName);
            //service = new WordService(outName, false);
            service = new WordService(toFileName, false);
        }


        protected bool isOutputExists()
        {
            return (!string.IsNullOrEmpty(outName) && File.Exists(outName));
        }



        private bool SaveAs(string path, string fileName)
        {
            return SaveAs(path + "//" + fileName);
        }


        private bool SaveAs(string toFullFileName)
        {
            try
            {
                File.Copy(FileArchiveTransport.GetTemplatesPath() + TEMPLATE_FILE_NAME, toFullFileName, true);
                outName = toFullFileName;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public abstract void PasteAllAttach();
        public abstract void PasteAttachToETSOrder();
        public abstract void PasteQualifications();
        public abstract void PasteTechSpecs();
        public abstract void PasteAgreements();        


        public void Close()
        {
            service.CloseWord(true);
        }
    }
}
