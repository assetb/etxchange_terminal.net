using altaik.baseapp.ext;

namespace AltaBO.specifics {
    public enum DocumentTypeEnum {
        [Name("Техническая спецификация")]
        TechSpecs = 1,

        [Name("Квалификационные требования")]
        Qualifications = 2,

        [Name("Проект договора")]
        Agreement = 3,

        [Name("Заявка на проведение")]
        Application = 4,

        [Name("Заявка от заказчика на обработку")]
        OrderSource = 5,

        [Name("Заявка от заказчика на биржу")]
        Order = 6,

        [Name("Заявка от поставщика на обработку")]
        SupplierOrderSource = 7,

        [Name("Заявка от поставщика")]
        SupplierOrder = 8,

        [Name("Список претендентов")]
        Applicants = 9,

        [Name("Счет ГО")]
        InvoiceGO = 10,

        [Name("Поручение на сделку")]
        ProcuratorySource = 11,

        [Name("Поручение от поставщика")]
        Procuratory = 12,

        [Name("Шаблон протокола")]
        ProtocolTemplate = 13,

        [Name("Протокол с биржи")]
        ProtocolSource = 14,

        [Name("Протокол для заказчика")]
        Protocol = 15,

        [Name("Отчет заказчику")]
        CustomerReport = 16,

        [Name("Отчет поставщику")]
        SupplierReport = 17,

        [Name("Счет на оплату")]
        Invoice = 18,

        [Name("Акт о несостоявшемся торге")]
        FailedAct = 19,

        [Name("Коммерческое предложение")]
        CommercialOffer = 20,

        [Name("Договор")]
        Contract = 21,

        [Name("Прочее")]
        Other = 22,

        [Name("Заявка от заказчика (оригинал)")]
        OrderOriginal = 23,

        [Name("Проект договора от заказчика на обработку")]
        AgreementSource = 24,

        [Name("Схема")]
        Scheme = 25,

        [Name("Договор между заказчиком и поставщиком")]
        ContractBetweenCS = 26,

        [Name("Договор на поставку товара")]
        SupplyGoodContract = 27,

        [Name("Сопроводительное письмо")]
        CoverLetter = 28,

        [Name("Заявление об изменении денежных средств")]
        MoneyTransfer = 29,

        [Name("Анкета компании")]
        CompanyProfile = 30,

        // Post document types
        [Name("Счет-фактура")]
        InvoiceFacture = 31,

        [Name("Акт выполненных работ(услуг)")]
        CertificateOfCompletion = 32,

        [Name("Акт сверки")]
        ActOfReconcilation = 33,

        [Name("Заявка на аукцион")]
        ApplicationForAuction = 34,

        [Name("Отчет об исполнении поручения")]
        ReportAboutExecution = 35,

        [Name("Дополнительное соглашение")]
        SupplementaryAgreement = 36,

        [Name("Прочие договоры")]
        OtherAgreements = 37,

        [Name("Входящее письмо")]
        IncomingMail = 38,

        [Name("Исходящее письмо")]
        OutgoingMail = 39,

        [Name("Протокол аукциона")]
        AuctionProtocol = 40,

        [Name("Реестр сделок")]
        RegisterOfTransactions = 41,

        [Name("Доверенность")]
        PowerOfAttorney = 42,

        //
        [Name("Гарантийное письмо")]
        WarrantyLetter = 43,

        [Name("Список претендентов для заказчика")]
        ApplicantsForCustomer = 44,

        [Name("Документы к заявке на участие")]
        DocumentsForSupplierOrder = 45,

        [Name("Список участников от заказчика")]
        ApplicantsFromCustomer = 46,
    }
}