namespace DocumentFormationServices
{
    public static class DFConfig
    {
        public static string SupplierReportTemplateFileName { get; } = "reportToSupplier";
        public static string ClientReportTemplateFileName { get; } = "reportToClient";
        public static string SupplierReportFileName { get; } = "Отчет поставщику ";
        public static string ClientReportFileName { get; } = "Отчет клиенту по лоту ";
    }
}
