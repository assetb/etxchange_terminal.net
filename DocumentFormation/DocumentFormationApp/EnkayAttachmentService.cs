using System;
using altaik.baseapp.ext;
using DocumentFormation.model;

namespace DocumentFormation
{
    public class EnkayAttachmentService:CustomerAttachmentService
    {
        public EnkayAttachmentService(string file) : base(file) {
            TEMPLATE_FILE_NAME = "OrderAttach.docx";
        }

        public override void PasteAgreements()
        {
            throw new NotImplementedException();
        }

        public override void PasteAllAttach()
        {
            throw new NotImplementedException();
        }

        public override void PasteAttachToETSOrder()
        {
            try {
                service.PasteBookmark(DocumentTypeEnum.Qualifications.GetName().Replace(" ", "_"));
                service.CorrectAfterPaste(DocumentTypeEnum.Qualifications.GetName().Replace(" ", "_"), new string[3] { DocumentTypeEnum.Agreement.GetName().Replace(" ", "_"), DocumentTypeEnum.Qualifications.GetName().Replace(" ", "_"), DocumentTypeEnum.TechSpecs.GetName().Replace(" ", "_") });
            } catch { }
        }

        public override void PasteQualifications()
        {
            throw new NotImplementedException();
        }

        public override void PasteTechSpecs()
        {
            throw new NotImplementedException();
        }
    }
}
