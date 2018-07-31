using AltaBO;
using DocumentFormation.model;

namespace DocumentFormation
{
    /// <summary>
    /// Abstract class to operate with customer incoming file to get Order.
    /// It uses windows buffer to copy attachments to output files.
    /// </summary>
    public abstract class CustomerOrderService
    {

        public abstract Order UpdateOrder(Order order);

        public bool CopyAttachment(DocumentTypeEnum bookmark)
        {
            var res = false;
            switch (bookmark)
            {
                case (DocumentTypeEnum.Agreement): { res = CopyAgreement(); } break;
                case (DocumentTypeEnum.TechSpecs): { res = CopyTechSpecs(); } break;
                case (DocumentTypeEnum.Qualifications): { res = CopyQualificationsToBuffer(); } break;
            }

            return res;
        }


        public abstract bool CopyAgreement();
        public abstract bool CopyTechSpecs();
        public abstract bool CopyTechSpecs(int sheetIndex);
        public abstract bool CopyQualificationsToBuffer();

        public abstract void Close();
    }
}
