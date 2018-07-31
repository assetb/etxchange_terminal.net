using System;
using altaik.baseapp.ext;
using DocumentFormation.model;

namespace DocumentFormation
{
    public class VostokAttachmentService : CustomerAttachmentService
    {
        public VostokAttachmentService(string file) : base(file)
        {
            TEMPLATE_FILE_NAME = "OrderAttach.docx";
        }

        public override void PasteAllAttach()
        {
            throw new NotImplementedException();
        }

        public override void PasteAttachToETSOrder() {
            throw new NotImplementedException();
        }

        public override void PasteQualifications()
        {
            service.PasteBookmark(DocumentTypeEnum.Qualifications.GetName().Replace(" ", "_"));
        }

        public override void PasteTechSpecs()
        {
            service.PasteBookmark(DocumentTypeEnum.TechSpecs.GetName().Replace(" ", "_"));
        }

        public override void PasteAgreements()
        {
            service.PasteBookmark(DocumentTypeEnum.Agreement.GetName().Replace(" ", "_"));
        }
    }
}
