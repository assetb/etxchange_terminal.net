using altaik.baseapp.vm;

namespace AltaBO {
    public class Applicant:BaseViewModel {
        public int Id { get; set; }
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
    }
}
