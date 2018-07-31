using AltaOffice;
using AltaTransport;
using System;
using System.IO;

namespace DocumentFormation.app
{
    public abstract class AttachmentService
    {
        protected string TEMPLATE_FILE_NAME = "order_ets_attach.docx";
        public WordService service;
        private string outName;


        public AttachmentService(string toFileName)
        {
            SaveAs(toFileName);
            service = new WordService(outName, false);
        }


        protected Boolean isOutputExists()
        {
            return (!string.IsNullOrEmpty(outName) && File.Exists(outName));
        }



        private Boolean SaveAs(string path, string fileName)
        {
            return SaveAs(path + "//" + fileName);
        }


        private Boolean SaveAs(string toFullFileName)
        {
            try
            {
                File.Copy(FileArchiveTransport.GetOrderTemplatePath() + TEMPLATE_FILE_NAME, toFullFileName, true);
                outName = toFullFileName;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public void Close()
        {
            service.CloseWord(true);
        }
    }
}
