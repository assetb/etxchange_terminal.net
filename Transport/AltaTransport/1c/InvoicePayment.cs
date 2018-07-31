#pragma warning disable 1591

namespace AltaTransport._1c.InvoicePaiment
{

    #region Протокол подключения к веб-сервису

    /// <summary>
    /// Реализует протокол подключения к веб-сервису.
    /// </summary>
    [System.Web.Services.WebServiceBindingAttribute(Name="InvoicePaymentSoapBinding", Namespace="http://localhost/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(InvoicePrint))]
    public partial class InvoicePaymentConnectionProtocol : System.Web.Services.Protocols.SoapHttpClientProtocol {

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        
        public InvoicePaymentConnectionProtocol(int connType)
        {
            // New server
            switch(connType) {
                case 1:
                    Url = "http://192.168.10.4/Altaik/ws/InvoicePayment";
                    Credentials = new System.Net.NetworkCredential("altaservice", "ikalta7&");
                    break;
                case 2:
                    Url = "http://192.168.10.4/Korund-777/ws/InvoicePayment";
                    Credentials = new System.Net.NetworkCredential("korundservice", "korund7&7");
                    break;
                case 3:
                    Url = "http://192.168.10.4/AkAltynKo/ws/InvoicePayment";
                    Credentials = new System.Net.NetworkCredential("akaltynservice", "AkAltynKo");
                    break;
            }
        }

        public InvoicePaymentConnectionProtocol(string Url, string Login, string Pass) {
            this.Url = Url;
            Credentials = new System.Net.NetworkCredential(Login, Pass);
        }
        /// <summary>
        /// Получает или задает URL-адрес веб-службы, запрашиваемой клиентом.
        /// </summary>
        public new string Url
        {
            get {
                return base.Url;
            }
            set {
                base.Url = value;
            }
        }

        /// <summary>
        /// Получает или задает учетные данные безопасности для проверки подлинности клиента веб-службы.
        /// </summary>
        public new System.Net.ICredentials Credentials
        {
            get {
                return base.Credentials;
            }
            set
            {
                base.Credentials = value;
            }
        }

        /// <summary>
        ///Получает или задает значение, указывающее, должно ли для свойства System.Web.Services.Protocols.WebClientProtocol.Credentials
        ///устанавливаться значение свойства System.Net.CredentialCache.DefaultCredentials.
        /// </summary>
        public new bool UseDefaultCredentials
        {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
            }
        }

        /// <summary>
        /// Выполняет поиск и чтение счета на оплату или счета на оплату ГО в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса InvoiceSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденный счет на оплату (экземпляр класса Invoice). В противном случае RequestSuccess = false,
        /// а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readInvoice", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readInvoice(InvoiceSearch SearchParam) {
            var results = Invoke("readInvoice", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запись/перезапиь счета на оплату или счета на оплату ГО в базе 1С.
        /// </summary>
        /// <param name="Payment">
        /// Объект для записи (экземпляр класса Invoice).
        /// </param>
        /// <param name="canRewrite">
        /// true если перезаписывать счета на оплату, в случае его существования в базе 1С, иначе false.
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <param name="GO">
        /// true если записать объект как счета на оплату гарантийного обязательства, 
        /// иначе false (записывать как счет на оплату)
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит поля поиска счета на оплату (InvoiceSearch). 
        /// Иначе RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeInvoice", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeInvoice(Invoice Payment, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> canRewrite, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> GO) {
            var results = Invoke("writeInvoice", new object[] {
                        Payment,
                        canRewrite,
                        GO});
            return ((ResponseData)(results[0]));
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readInvoiceEx", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readInvoiceEx(InvoiceSearch SearchParam)
        {
            var results = Invoke("readInvoiceEx", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeInvoiceEx", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeInvoiceEx(InvoiceEx Payment, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] System.Nullable<bool> canRewrite, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] System.Nullable<bool> GO)
        {
            var results = Invoke("writeInvoiceEx", new object[] {
                        Payment,
                        canRewrite,
                        GO});
            return ((ResponseData)(results[0]));
        }


        /// <summary>
        /// Выполняет поиск и чтение печатной формы счета на оплату или печатной формы счета на оплату ГО в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса InvoiceSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденную печатную форму счета на оплату (экземпляр класса InvoicePrint).
        /// В противном случае RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:printInvoice", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData printInvoice(InvoiceSearch SearchParam) {
            var results = Invoke("printInvoice", new object[] { SearchParam });
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет поиск и чтение контрагента в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса ClientSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденного контрагента (экземпляр класса Clients). В противном случае RequestSuccess = false,
        /// а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readClient", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readClient(ClientSearch SearchParam) {
            var results = Invoke("readClient", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запись/перезапиь контрагента в базе 1С.
        /// </summary>
        /// <param name="Client">
        /// Объект для записи (экземпляр класса Clients).
        /// </param>
        /// <param name="canRewrite">
        /// true если перезаписывать контрагента, в случае его существования в базе 1С, иначе false.
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит поля поиска контрагента (ClientSearch). 
        /// Иначе RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeClient", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeClient(Clients Client, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> canRewrite) {
            var results = Invoke("writeClient", new object[] {
                        Client,
                        canRewrite});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запрос к справочнику контрагенты в базе 1С.
        /// </summary>
        /// <param name="Query">
        /// Строка 1С запроса.
        /// </param>
        /// <returns>
        /// Возвращает список контрагентов (массив объектов Clients), удоволетваряющих запросу.
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getClientList", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Client", Namespace="http://www.sample-package.org", IsNullable=false)]
        public Clients[] getClientList(string Query) {
            var results = Invoke("getClientList", new object[] {Query});
            return ((Clients[])(results[0]));
        }

        /// <summary>
        /// Выполняет поиск и чтение договора контрагента в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса ContractSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденного контрагента (экземпляр класса Contracts). В противном случае RequestSuccess = false,
        /// а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readContract", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readContract(ContractSearch SearchParam) {
            var results = Invoke("readContract", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запись/перезапиь договора контрагента в базе 1С.
        /// </summary>
        /// <param name="Contract">
        /// Объект для записи (экземпляр класса Contracts).
        /// </param>
        /// <param name="canRewrite">
        /// true если перезаписывать договор контрагента, в случае его существования в базе 1С, иначе false.
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит поля поиска договора контрагента (ContractSearch). 
        /// Иначе RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeContract", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeContract(Contracts Contract, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> canRewrite) {
            var results = Invoke("writeContract", new object[] {
                        Contract,
                        canRewrite});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запрос к справочнику договоры контрагентов в базе 1С.
        /// </summary>
        /// <param name="Query">
        /// Строка 1С запроса.
        /// </param>
        /// <returns>
        /// Возвращает список договоров контрагентов (массив объектов Contracts), удоволетваряющих запросу.
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getContractList", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Contract", Namespace="http://www.sample-package.org", IsNullable=false)]
        public Contracts[] getContractList(string Query) {
            var results = Invoke("getContractList", new object[] {Query});
            return ((Contracts[])(results[0]));
        }

        /// <summary>
        /// Выполняет поиск и чтение банковского счета в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса BankAccountSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденный банковский счет (экземпляр класса BankAccounts). В противном случае RequestSuccess = false,
        /// а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readBankAccount", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readBankAccount(BankAccountSearch SearchParam) {
            var results = Invoke("readBankAccount", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запись/перезапиь банковского счета в базе 1С.
        /// </summary>
        /// <param name="BankAccount">
        /// Объект для записи (экземпляр класса BankAccounts).
        /// </param>
        /// <param name="canRewrite">
        /// true если перезаписывать банковский счет, в случае его существования в базе 1С, иначе false.
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит поля поиска банковского счета (BankAccountSearch). 
        /// Иначе RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeBankAccount", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeBankAccount(BankAccounts BankAccount, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> canRewrite) {
            var results = Invoke("writeBankAccount", new object[] {
                        BankAccount,
                        canRewrite});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запрос к справочнику банковские счета в базе 1С.
        /// </summary>
        /// <param name="Query">
        /// Строка 1С запроса.
        /// </param>
        /// <returns>
        /// Возвращает список банковских счетов (массив объектов BankAccounts), удоволетваряющих запросу.
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getBankAccountList", RequestNamespace="http://localhost/", ResponseNamespace="http://localhost/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("BankAccount", Namespace="http://www.sample-package.org", IsNullable=false)]
        public BankAccounts[] getBankAccountList(string Query) {
            var results = Invoke("getBankAccountList", new object[] {Query});
            return ((BankAccounts[])(results[0]));
        }

        /// <summary>
        /// Выполняет поиск и чтение банка в базе 1С.
        /// </summary>
        /// <param name="SearchParam">
        /// Экземпляр класса BankSearch, по полям которого осуществляется поиск.
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит
        /// найденный банк (экземпляр класса Bank). В противном случае RequestSuccess = false,
        /// а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:readBank", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData readBank(BankSearch SearchParam) {
            var results = Invoke("readBank", new object[] {SearchParam});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запись/перезапиь банка в базе 1С.
        /// </summary>
        /// <param name="Bank">
        /// Объект для записи (экземпляр класса Banks).
        /// </param>
        /// <param name="canRewrite">
        /// true если перезаписывать банк, в случае его существования в базе 1С, иначе false.
        /// <para>Значение по умолчанию false.</para>
        /// </param>
        /// <returns>
        /// Экземпляр класса ResponseObject.
        /// <para>В случае успеха свойство RequestSuccess = true, а свойство ResponseObject содержит поля поиска банка (BankSearch). 
        /// Иначе RequestSuccess = false, а свойство ErrorMsg содержит описание ошибки.</para>
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:writeBank", RequestNamespace="http://localhost/", ResponseNamespace="http://localhost/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResponseData writeBank(Banks Bank, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<bool> canRewrite) {
            var results = Invoke("writeBank", new object[] {
                        Bank,
                        canRewrite});
            return ((ResponseData)(results[0]));
        }

        /// <summary>
        /// Выполняет запрос к справочнику банки в базе 1С.
        /// </summary>
        /// <param name="Query">
        /// Строка 1С запроса.
        /// </param>
        /// <returns>
        /// Возвращает список банков (массив объектов Banks), удоволетваряющих запросу.
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getBankList", RequestNamespace="http://localhost/", ResponseNamespace="http://localhost/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Bank", Namespace="http://www.sample-package.org", IsNullable=false)]
        public Banks[] getBankList(string Query) {
            var results = Invoke("getBankList", new object[] {Query});
            return ((Banks[])(results[0]));
        }

        /// <summary>
        /// Получает акт сверки взаиморасчитов с контрагентом.
        /// </summary>
        /// <param name="ReportSelection">
        /// Экземпляр класса RRSelection, по полям которого осуществляется отбор акта сверки взаиморасчетов.
        /// </param>
        /// <returns>
        /// Строки таблицы акта сверки взаиморасчетов с контрагентом (массив объектов ReconciliationReportRow).
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getReconciliationReport", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Row", Namespace="http://www.sample-package.org", IsNullable=false)]
        public ReconciliationReportRow[] getReconciliationReport(RRSelection ReportSelection) {
            var results = Invoke("getReconciliationReport", new object[] {ReportSelection});
            return ((ReconciliationReportRow[])(results[0]));
        }

        /// <summary>
        /// Получает отчет по дебиторской задолженности контрагента.
        /// </summary>
        /// <param name="ReportSelection">
        /// Экземпляр класса RRSelection, по полям которого осуществляется отбор отчета.
        /// </param>
        /// <returns>
        /// Строки таблицы отчета по дебиторской задолженности (массив объектов ReconciliationReportRow).
        /// </returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://localhost/#InvoicePayment:getDebitorsReport", RequestNamespace = "http://localhost/", ResponseNamespace = "http://localhost/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Row", Namespace="http://www.sample-package.org", IsNullable=false)]
        public ReconciliationReportRow[] getDebitorsReport(RRSelection ReportSelection) {
            var results = Invoke("getDebitorsReport", new object[] {ReportSelection});
            return ((ReconciliationReportRow[])(results[0]));
        }
        
    }

    /// <summary>
    /// Описывает результат выполнения методов веб-сервиса: readInvoice, writeInvoice, printInvoice, readClient, 
    ///  writeClient, readContract, writeContract, readBankAccount, writeBankAccount, readBank и writeBank.
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class ResponseData
    {

        private bool requestSuccessField;
        private string errorMsgField;
        private object responseObjectField;

        /// <summary>
        /// true если запрос выполнен успешно, иначе false.
        /// </summary>
        public bool RequestSuccess
        {
            get
            {
                return requestSuccessField;
            }
            set
            {
                requestSuccessField = value;
            }
        }

        /// <summary>
        /// Содержит описание ошибки если RequestSuccess=false.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ErrorMsg
        {
            get
            {
                return errorMsgField;
            }
            set
            {
                errorMsgField = value;
            }
        }

        /// <summary>
        /// Если RequestSuccess=true то содержит либо прочитанный объект, либо поисковые реквизита записанного объекта.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ResponseObject
        {
            get
            {
                return responseObjectField;
            }
            set
            {
                responseObjectField = value;
            }
        }
    }

#endregion
    
#region Классы для работы с документом счет на оплату

    /// <summary>
    /// Описывает поля по кторым происходит поиск счета на оплату.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса передается методам веб-сервиса readInvoice и printInvoice.
    /// Так же экземпляр данного класса возвращается свойством ResponseObject класса ResponseData при записи счета на оплату.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.sample-package.org")]
    public partial class InvoiceSearch {
        
        private System.DateTime docDateField;
        private string docNumField;
        private System.Nullable<bool> goField;

        /// <summary>
        /// Дата искомого счета на оплату.
        /// </summary>
        /// <remarks>
        /// <para>Поле обязательно к заполнению.</para>
        /// <para>Дата устанавливается с периодичностью до года, т.к. номер обеспечивает уникальность счета на оплату в разрезе года.</para>
        /// <para>Например, для поиска счета на оплату от 30.09.2016 №00000000031, дату можно инициализировать любым значением в интервале от 01.01.2016 до 31.12.2016:</para>
        /// <code>
        /// <para>invoiceSearch.DocDate = new DateTime(2016, 01, 01);</para>
        /// <para>invoiceSearch.DocDate = new DateTime(2016, 09, 30);</para> 
        /// <para>invoiceSearch.DocDate = new DateTime(2016, 12, 31);</para> 
        /// </code>
        /// <para>Все три варианта одинаково верны.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DocDate {
            get {
                return docDateField;
            }
            set {
                docDateField = value;
            }
        }

        /// <summary>
        /// Номер искомого счета на оплату. Строка 11 символов.
        /// </summary>
        /// <remarks>
        /// <para>Поле обязательно к заполнению.</para>
        /// <para>Номер необходимо дозаполнять лидирующими нулями слева.</para>
        /// <para>Например, для поиска счета на оплату от 30.09.2016 №00000000031, Номер можно задать следующим образом:</para>
        /// <code>
        /// <para>int iDocNumber = 31;</para>
        /// <para>string sDocNumber = "31";</para>
        /// <para>string sFullDocNumber = "00000000031";</para>
        /// <para>invoiceSearch.DocNum = iDocNumber.ToString().PadLeft(11,'0');</para>
        /// <para>invoiceSearch.DocNum = sDocNumber.PadLeft(11,'0');</para>
        /// <para>invoiceSearch.DocNum = sFullDocNumber;</para>
        /// </code>
        /// <para>Все три варианта одинаково верны.</para>
        /// <para>P.S.</para>
        /// <para>Дозаполнение лидирующими нулями можно поместить в аксессор set свойства DocNum:</para>
        /// <code>
        ///         public string DocNum
        ///        {
        ///            get {
        ///                return this.docNumField;
        ///            }
        ///            set {
        ///                this.docNumField = value.PadLeft(11,'0');
        ///            }
        ///        }
        /// </code>
        /// </remarks>
        public string DocNum
        {
            get {
                return docNumField;
            }
            set {
                docNumField = value;
            }
        }

        /// <summary>
        /// Тип искомого счета на оплату.
        /// </summary>
        /// <remarks>
        /// Если true тогда поиск производится по счетам на оплату гарантийного обязательства, в противном случае поиск производится по счетам на оплату.
        /// <para>Не обязательно к заполнению: по умолчанию false.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> GO {
            get {
                return goField;
            }
            set {
                goField = value;
            }
        }
}

    /// <summary>
    /// Представляет печатную форму счета на оплату.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске печатной формы счета на оплату.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class InvoicePrint
    {

        private string supplierField;
        private string bINField;
        private string bankAccountField;
        private string kBEField;
        private string bankField;
        private string bIKField;
        private string paymentCodeField;
        private string headerField;
        private string supplierContentField;
        private string clientContentField;
        private string contractField;
        private string rowNumberField;
        private string codeField;
        private string serviceField;
        private string quantityField;
        private string unitField;
        private string priceField;
        private string аmountField;
        private string totalField;
        private string totalTaxField;
        private string аmountInWordsField;
        private string footerField;
        private string contractorField;

        /// <summary>
        /// Заголовок печатной формы.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Header
        {
            get
            {
                return headerField;
            }
            set
            {
                headerField = value;
            }
        }

        /// <summary>
        /// Поставщик услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Supplier
        {
            get
            {
                return supplierField;
            }
            set
            {
                supplierField = value;
            }
        }

        /// <summary>
        /// БИН/ИИК поставщика.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string BIN
        {
            get
            {
                return bINField;
            }
            set
            {
                bINField = value;
            }
        }


        /// <summary>
        /// КБЕ поставщика.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string KBE
        {
            get
            {
                return kBEField;
            }
            set
            {
                kBEField = value;
            }
        }

        /// <summary>
        /// Банк.																	
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Bank
        {
            get
            {
                return bankField;
            }
            set
            {
                bankField = value;
            }
        }

        /// <summary>
        /// БИК банка.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string BIK
        {
            get
            {
                return bIKField;
            }
            set
            {
                bIKField = value;
            }
        }

        /// <summary>
        /// Банковский счет.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string BankAccount
        {
            get
            {
                return bankAccountField;
            }
            set
            {
                bankAccountField = value;
            }
        }

        /// <summary>
        /// Код назначения платежа.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PaymentCode
        {
            get
            {
                return paymentCodeField;
            }
            set
            {
                paymentCodeField = value;
            }
        }

        /// <summary>
        /// Реквизиты поставщика.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string SupplierContent
        {
            get
            {
                return supplierContentField;
            }
            set
            {
                supplierContentField = value;
            }
        }

        /// <summary>
        /// Реквизиты покупателя.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ClientContent
        {
            get
            {
                return clientContentField;
            }
            set
            {
                clientContentField = value;
            }
        }

        /// <summary>
        /// Договор на оплату.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Contract
        {
            get
            {
                return contractField;
            }
            set
            {
                contractField = value;
            }
        }

        /// <summary>
        /// Номер строки табличной части услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string RowNumber
        {
            get
            {
                return rowNumberField;
            }
            set
            {
                rowNumberField = value;
            }
        }

        /// <summary>
        /// Код услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        /// <summary>
        /// Наименование услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Service
        {
            get
            {
                return serviceField;
            }
            set
            {
                serviceField = value;
            }
        }

        /// <summary>
        /// Количество услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Quantity
        {
            get
            {
                return quantityField;
            }
            set
            {
                quantityField = value;
            }
        }

        /// <summary>
        /// Единица измерения услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Unit
        {
            get
            {
                return unitField;
            }
            set
            {
                unitField = value;
            }
        }

        /// <summary>
        /// Цена услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Price
        {
            get
            {
                return priceField;
            }
            set
            {
                priceField = value;
            }
        }

        /// <summary>
        /// Сумма услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Аmount
        {
            get
            {
                return аmountField;
            }
            set
            {
                аmountField = value;
            }
        }

        /// <summary>
        /// Итоговая сумма.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Total
        {
            get
            {
                return totalField;
            }
            set
            {
                totalField = value;
            }
        }

        /// <summary>
        /// Итоговая сумма НДС.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string TotalTax
        {
            get
            {
                return totalTaxField;
            }
            set
            {
                totalTaxField = value;
            }
        }

        /// <summary>
        /// Итоговая сумма прописью.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string АmountInWords
        {
            get
            {
                return аmountInWordsField;
            }
            set
            {
                аmountInWordsField = value;
            }
        }

        /// <summary>
        /// Подвал печатной формы.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Footer
        {
            get
            {
                return footerField;
            }
            set
            {
                footerField = value;
            }
        }

        /// <summary>
        /// ФИО исполнителя для подписи.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Contractor
        {
            get
            {
                return contractorField;
            }
            set
            {
                contractorField = value;
            }
        }
    }

    /// <summary>
    /// Представляет документ счет на оплату.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске счета на оплату.
    /// Так же экземпляр данного класса передается методу веб-сервиса writeInvoice при записи счета на оплату.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class Invoice
    {

        private System.DateTime docDateField;
        private string docNumField;
        private string paymentCodeField;
        private BankAccounts organizationBankAccountField;
        private Clients clientField;
        private Contracts contractField;
        private string serviceField;
        private string serviceContentField;
        private System.Nullable<float> quantityField;
        private System.Nullable<float> priceField;
        private System.Nullable<float> аmountField;
        private string commentsField;

        /// <summary>
        /// Дата документа.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// <para>При записи документа в базу 1С необходимо заполнять данное свойство точной датой документа.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DocDate
        {
            get
            {
                return docDateField;
            }
            set
            {
                docDateField = value;
            }
        }

        /// <summary>
        /// Номер документа. Строка 11 символов.
        /// </summary>
        /// <remarks>Поле обязательно к заполнению.</remarks>
        public string DocNum
        {
            get
            {
                return docNumField;
            }
            set
            {
                docNumField = value;
            }
        }

        /// <summary>
        /// Код назначения платежа. Строка 3 символа.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PaymentCode
        {
            get
            {
                return paymentCodeField;
            }
            set
            {
                paymentCodeField = value;
            }
        }

        /// <summary>
        /// Банковский счет.
        /// </summary>
        /// <remarks>
        /// В случае если банковский счет уже есть в базе 1С можно заполнить только поисковые реквизиты.
        /// <para>Например перед записью документа, отдельно был записан банковский счет.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public BankAccounts OrganizationBankAccount
        {
            get
            {
                return organizationBankAccountField;
            }
            set
            {
                organizationBankAccountField = value;
            }
        }

        /// <summary>
        /// Контрагент.
        /// </summary>
        /// <remarks>
        /// В случае если контрагент уже есть в базе 1С можно заполнить только поисковые реквизиты.
        /// <para>Например перед записью документа, отдельно был записан контрагент.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Clients Client
        {
            get
            {
                return clientField;
            }
            set
            {
                clientField = value;
            }
        }

        /// <summary>
        /// Договор контрагента.
        /// </summary>
        /// <remarks>
        /// В случае если договор контрагента уже есть в базе 1С можно заполнить только поисковые реквизиты.
        /// <para>Например перед записью документа, отдельно был записан договор контрагента.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Contracts Contract
        {
            get
            {
                return contractField;
            }
            set
            {
                contractField = value;
            }
        }

        /// <summary>
        /// Наименование услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Service
        {
            get
            {
                return serviceField;
            }
            set
            {
                serviceField = value;
            }
        }

        /// <summary>
        /// Содержание услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ServiceContent
        {
            get
            {
                return serviceContentField;
            }
            set
            {
                serviceContentField = value;
            }
        }

        /// <summary>
        /// Количество услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Quantity
        {
            get
            {
                return quantityField;
            }
            set
            {
                quantityField = value;
            }
        }

        /// <summary>
        /// Цена услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Price
        {
            get
            {
                return priceField;
            }
            set
            {
                priceField = value;
            }
        }

        /// <summary>
        /// Сумма услуги.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Аmount
        {
            get
            {
                return аmountField;
            }
            set
            {
                аmountField = value;
            }
        }

        /// <summary>
        /// Комментарий к документу.
        /// </summary>
        /// <remarks>
        /// Т.к. с базой 1С независимо обмениваются 3 программных системы, есть предложение в комментарий записывать 
        /// из какой системы был создан объект и дату записи/перезаписи. Чтобы в случае дублирования данных можно 
        /// было выяснить кто и когда записал данный объект.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Comments
        {
            get
            {
                return commentsField;
            }
            set
            {
                commentsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class ServicesRow
    {

        private string serviceField;
        private string serviceContentField;
        private System.Nullable<float> quantityField;
        private System.Nullable<float> priceField;
        private System.Nullable<float> аmountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Service
        {
            get
            {
                return serviceField;
            }
            set
            {
                serviceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ServiceContent
        {
            get
            {
                return serviceContentField;
            }
            set
            {
                serviceContentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Quantity
        {
            get
            {
                return quantityField;
            }
            set
            {
                quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Price
        {
            get
            {
                return priceField;
            }
            set
            {
                priceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Аmount
        {
            get
            {
                return аmountField;
            }
            set
            {
                аmountField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class InvoiceEx
    {

        private System.DateTime docDateField;
        private string docNumField;
        private string paymentCodeField;
        private BankAccounts organizationBankAccountField;
        private Clients clientField;
        private Contracts contractField;
        private ServicesRow[] servicesField;
        private string commentsField;
        private bool includeVATField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DocDate
        {
            get
            {
                return docDateField;
            }
            set
            {
                docDateField = value;
            }
        }

        /// <remarks/>
        public string DocNum
        {
            get
            {
                return docNumField;
            }
            set
            {
                docNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PaymentCode
        {
            get
            {
                return paymentCodeField;
            }
            set
            {
                paymentCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public BankAccounts OrganizationBankAccount
        {
            get
            {
                return organizationBankAccountField;
            }
            set
            {
                organizationBankAccountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Clients Client
        {
            get
            {
                return clientField;
            }
            set
            {
                clientField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public Contracts Contract
        {
            get
            {
                return contractField;
            }
            set
            {
                contractField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Services", IsNullable = true)]
        public ServicesRow[] Services
        {
            get
            {
                return servicesField;
            }
            set
            {
                servicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Comments
        {
            get
            {
                return commentsField;
            }
            set
            {
                commentsField = value;
            }
        }
        public bool IncludeVAT {
            get { return this.includeVATField; }
            set { this.includeVATField = value; }
        }
    }

    #endregion

    #region Классы для работы со справочником контрагенты

    /// <summary>
    /// Описывает поля по кторым происходит поиск справочника контрагенты.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса передается методу веб-сервиса readClient.
    /// Так же экземпляр данного класса возвращается свойством ResponseObject класса ResponseData при записи клиента.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class ClientSearch
    {

        private string iINField;
        private string ctlgCodeField;

        /// <summary>
        /// БИН/ИИН контрагента. Строка 12 символов.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        public string IIN
        {
            get
            {
                return iINField;
            }
            set
            {
                iINField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника контрагенты.
        /// </summary>
        /// <remarks>
        /// Поле не обязательно к заполнению, т.к. не является поисковым.
        /// <para>При записи контрагента содержит код нового элемента справочника контрагенты.</para>
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }
    }

    /// <summary>
    /// Представляет справочник контрагенты.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске контрагента.
    /// Так же экземпляр данного класса передается методу веб-сервиса writeClient при создании нового контрагента в базе 1С.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class Clients
    {

        private string ctlgNameField;
        private string ctlgCodeField;
        private string fullNameField;
        private string legalNaturaPersonField;
        private string countryCodeField;
        private string iINField;
        private string kBEField;
        private string phoneNumberField;
        private string аctualАddressField;
        private string legalAddressField;
        private string commentsField;
        private string parentCodeField;

        /// <summary>
        /// Наименование элемента справочника контрагенты.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgName
        {
            get
            {
                return ctlgNameField;
            }
            set
            {
                ctlgNameField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника контрагенты.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }

        /// <summary>
        /// Наименование контрагента.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string FullName
        {
            get
            {
                return fullNameField;
            }
            set
            {
                fullNameField = value;
            }
        }

        /// <summary>
        /// Юр./Физ. лицо. Строка либо 'ФизЛицо', либо 'ЮрЛицо' (регистр символов не важен).
        /// </summary>
        /// <remarks>
        /// <para>P.S.</para>
        /// Можно определить более удобное свойство с аксессорами get/set, главное, чтобы в поле legalNaturaPersonField попадала строка 'ФизЛицо', либо 'ЮрЛицо'.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string LegalNaturaPerson
        {
            get
            {
                return legalNaturaPersonField;
            }
            set
            {
                legalNaturaPersonField = value;
            }
        }

        /// <summary>
        /// Код страны резеденства. Строка 3 символа.
        /// </summary>
        /// <remarks>
        /// Код страны в ISO кодировке (КАЗАХСТАН - 398, РОССИЯ - 643, США - 840 и т.д.)
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CountryCode
        {
            get
            {
                return countryCodeField;
            }
            set
            {
                countryCodeField = value;
            }
        }

        /// <summary>
        /// БИН/ИИН контрагента. Строка 12 символов.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        public string IIN
        {
            get
            {
                return iINField;
            }
            set
            {
                iINField = value;
            }
        }

        /// <summary>
        /// КБЕ контрагента. Строка 2 символа.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string KBE
        {
            get
            {
                return kBEField;
            }
            set
            {
                kBEField = value;
            }
        }

        /// <summary>
        /// Телефон.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PhoneNumber
        {
            get
            {
                return phoneNumberField;
            }
            set
            {
                phoneNumberField = value;
            }
        }

        /// <summary>
        /// Фактический адрес контрагента.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string АctualАddress
        {
            get
            {
                return аctualАddressField;
            }
            set
            {
                аctualАddressField = value;
            }
        }

        /// <summary>
        /// Юридический адрес контрагента.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string LegalAddress
        {
            get
            {
                return legalAddressField;
            }
            set
            {
                legalAddressField = value;
            }
        }

        /// <summary>
        /// Комментарий к справочнику.
        /// </summary>
        /// <remarks>
        /// Т.к. с базой 1С независимо обмениваются 3 программных системы, есть предложение в комментарий записывать 
        /// из какой системы был создан объект и дату записи/перезаписи. Чтобы в случае дублирования данных можно 
        /// было выяснить кто и когда записал данный объект.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Comments
        {
            get
            {
                return commentsField;
            }
            set
            {
                commentsField = value;
            }
        }

        /// <summary>
        /// Архаизм.
        /// </summary>
        /// <remarks>
        /// Справочник контрагентов имеет иерархическую структуры.
        /// Для того, чтобы логически разделить контрагентов созданных Сириусом, контрагентов созданных АИС Альтой и контрагентов,
        /// создаваемых пользователями 1С, в каждую базу 1С был добавлен элемент верхнего уровня куда Сириус записывал новых контрагентов.
        /// В данном поле передается код такого элемента.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ParentCode
        {
            get
            {
                return parentCodeField;
            }
            set
            {
                parentCodeField = value;
            }
        }
    }

#endregion

#region Классы для работы со справочником договоры контрагентов

    /// <summary>
    /// Описывает поля по кторым происходит поиск справочника договоры контрагентов.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса передается методу веб-сервиса readContract.
    /// Так же экземпляр данного класса возвращается свойством ResponseObject класса ResponseData при записи договора контрагента.
    /// <para>Элеметы справочника полностью идентифицируются полями 'ИИН Контрагента' и 'Номер договора'.
    /// Но так как АИС Альта не заполняет поле номер договора, а помещает его в наименование элемента справочника договоры контрагентов,
    /// то пришлось усложнить схему поиска договоров: Если по номеру и владельцу не удается найти договор, то производится попытка найти номеру в наименовании.</para>
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class ContractSearch
    {

        private string clientIINField;
        private string contractNumberField;
        private string ctlgNameField;
        private string ctlgCodeField;

        /// <summary>
        /// БИН/ИИН контрагента. Строка 12 символов.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        public string ClientIIN
        {
            get
            {
                return clientIINField;
            }
            set
            {
                clientIINField = value;
            }
        }

        /// <summary>
        /// Номер договора.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        public string ContractNumber
        {
            get
            {
                return contractNumberField;
            }
            set
            {
                contractNumberField = value;
            }
        }

        /// <summary>
        /// Наименование элемента справочника договоры контрагентов.
        /// </summary>
        /// <remarks>
        /// Не обязательное поле. При записи возвращает наименование созданного элемента.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgName
        {
            get
            {
                return ctlgNameField;
            }
            set
            {
                ctlgNameField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника договоры контрагентов.
        /// </summary>
        /// <remarks>
        /// Не обязательное поле. При записи возвращает код созданного элемента.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }
    }

    /// <summary>
    /// Представляет справочник договоры контрагентов.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске договора контрагента.
    /// Так же экземпляр данного класса передается методу веб-сервиса writeContract при создании нового договора контрагента в базе 1С.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class Contracts
    {

        private string ctlgNameField;
        private string ctlgCodeField;
        private string clientIINField;
        private string contractNumberField;
        private System.Nullable<System.DateTime> contracDateField;
        private string contractTypeField;
        private string settlementsField;
        private System.Nullable<System.DateTime> startDateField;
        private System.Nullable<System.DateTime> terminationDateField;
        private string сontractСurrencyField;
        private string commentsField;

        /// <summary>
        /// Наименование элемента справочника договоры контрагентов.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgName
        {
            get
            {
                return ctlgNameField;
            }
            set
            {
                ctlgNameField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника договоры контрагентов.
        /// </summary>
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }

        /// <summary>
        /// БИН/ИИН контрагента. Строка 12 символов.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        public string ClientIIN
        {
            get
            {
                return clientIINField;
            }
            set
            {
                clientIINField = value;
            }
        }

        /// <summary>
        /// Номер договора.
        /// </summary>
        /// <remarks>
        /// Поле обязательно к заполнению.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ContractNumber
        {
            get
            {
                return contractNumberField;
            }
            set
            {
                contractNumberField = value;
            }
        }

        /// <summary>
        /// Дата договора.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> ContracDate
        {
            get
            {
                return contracDateField;
            }
            set
            {
                contracDateField = value;
            }
        }

        /// <summary>
        /// Вид договора.
        /// </summary>
        /// <remarks>
        /// Строка либо 'СПоставщиком', либо 'СПокупателем', либо 'Прочее' (Не зависит от регистра символов).
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ContractType
        {
            get
            {
                return contractTypeField;
            }
            set
            {
                contractTypeField = value;
            }
        }

        /// <summary>
        /// Ведение взаиморасчетов.
        /// </summary>
        /// <remarks>
        /// Строка либо 'ПоДоговоруВЦелом', либо 'ПоРасчетнымДокументам' (Не зависит от регистра символов).
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Settlements
        {
            get
            {
                return settlementsField;
            }
            set
            {
                settlementsField = value;
            }
        }

        /// <summary>
        /// Дата начала действия договора.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> StartDate
        {
            get
            {
                return startDateField;
            }
            set
            {
                startDateField = value;
            }
        }

        /// <summary>
        /// Дата окончания действия договора.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> TerminationDate
        {
            get
            {
                return terminationDateField;
            }
            set
            {
                terminationDateField = value;
            }
        }

        /// <summary>
        /// Комментарий к справочнику.
        /// </summary>
        /// <remarks>
        /// Т.к. с базой 1С независимо обмениваются 3 программных системы, есть предложение в комментарий записывать 
        /// из какой системы был создан объект и дату записи/перезаписи. Чтобы в случае дублирования данных можно 
        /// было выяснить кто и когда записал данный объект.
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Comments
        {
            get
            {
                return commentsField;
            }
            set
            {
                commentsField = value;
            }
        }

        /// <summary>
        /// Валюта взаиморасчетов по договору.
        /// </summary>
        /// <remarks>
        /// Строка 3 символа в кодировке ISO-4217 (KZT, RUB, USD и т.д.)
        /// </remarks>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string СontractСurrency
        {
            get
            {
                return сontractСurrencyField;
            }
            set
            {
                сontractСurrencyField = value;
            }
        }

    }

#endregion

#region Классы для работы со справочником банки

    /// <summary>
    /// Описывает поля по кторым происходит поиск справочника банки.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса передается методу веб-сервиса readBank.
    /// Так же экземпляр данного класса возвращается свойством ResponseObject класса ResponseData при записи банка.
    /// <para>С синхронизацией банков дела обстоят сложнее. Т.к. могут существовать два элемента 
    /// с одним банковским идентификационным кодом (банк и его филиал могут иметь один и тот же БИК), 
    /// а БИН/ИИН в базе 1С не всегда заполнен, то в качастве разрешения коллизий был выбран код 
    /// элемента справочника банки из 1С.</para>
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class BankSearch
    {

        private string bIKField;
        private string ctlgCodeField;

        /// <summary>
        /// БИК банка. Строка 11 символов.
        /// </summary>
        public string BIK
        {
            get
            {
                return bIKField;
            }
            set
            {
                bIKField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника банки.
        /// </summary>
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }
    }

    /// <summary>
    /// Представляет справочник банки.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске банка.
    /// Так же экземпляр данного класса передается методу веб-сервиса writeBank при создании нового банка в базе 1С.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class Banks
    {

        private string ctlgNameField;
        private string ctlgCodeField;
        private string bINField;
        private string bIKField;
        private string cityField;
        private string phoneNumberField;
        private string addressField;

        /// <summary>
        /// Наименование элемента справочника банки.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgName
        {
            get
            {
                return ctlgNameField;
            }
            set
            {
                ctlgNameField = value;
            }
        }

        /// <summary>
        /// Код элемента справочника банки.
        /// </summary>
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }

        /// <summary>
        /// БИН банка. Строка 11 символов.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string BIN
        {
            get
            {
                return bINField;
            }
            set
            {
                bINField = value;
            }
        }

        /// <summary>
        /// БИК банка. Строка 11 символов.
        /// </summary>
        public string BIK
        {
            get
            {
                return bIKField;
            }
            set
            {
                bIKField = value;
            }
        }

        /// <summary>
        /// Город.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string City
        {
            get
            {
                return cityField;
            }
            set
            {
                cityField = value;
            }
        }

        /// <summary>
        /// Телефон.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string PhoneNumber
        {
            get
            {
                return phoneNumberField;
            }
            set
            {
                phoneNumberField = value;
            }
        }

        /// <summary>
        /// Адрес.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Address
        {
            get
            {
                return addressField;
            }
            set
            {
                addressField = value;
            }
        }
    }

#endregion

#region Классы для работы со справочником банковские счета

    /// <summary>
    /// Описывает поля по кторым происходит поиск справочника банковские счета.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса передается методу веб-сервиса readBankAccount.
    /// Так же экземпляр данного класса возвращается свойством ResponseObject класса ResponseData при записи банковского счета.
    /// <para>Поиск банковских счетов ведется по номеру банковского счета и его владельцу.
    /// Владельцем может быть как контрагент, так и организация по которой ведется учет.
    /// В первом случае заполняется поле ClientIIN, во втором - OrganizationtIIN</para>
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class BankAccountSearch
    {

        private string clientIINField;
        private string organizationtIINField;
        private string accountNumberField;
        private string ctlgCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ClientIIN
        {
            get
            {
                return clientIINField;
            }
            set
            {
                clientIINField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string OrganizationtIIN
        {
            get
            {
                return organizationtIINField;
            }
            set
            {
                organizationtIINField = value;
            }
        }

        /// <remarks/>
        public string AccountNumber
        {
            get
            {
                return accountNumberField;
            }
            set
            {
                accountNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }
    }

    /// <summary>
    /// Представляет справочник банковские счета.
    /// </summary>
    /// <remarks>
    /// Экземпляр данного класса возвращается свойством ResponseObject класса ResponseData, при поиске банковского счета.
    /// Так же экземпляр данного класса передается методу веб-сервиса writeBankAccount при создании нового банковского счета в базе 1С.
    /// </remarks>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class BankAccounts
    {

        private string ctlgNameField;
        private string ctlgCodeField;
        private string clientIINField;
        private string organizationtIINField;
        private string bankBIКField;
        private string bankCodeField;
        private string accountNumberField;
        private string accountTypeField;
        private System.Nullable<System.DateTime> openingDateField;
        private System.Nullable<System.DateTime> closingDateField;
        private string сurrencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string CtlgName
        {
            get
            {
                return ctlgNameField;
            }
            set
            {
                ctlgNameField = value;
            }
        }

        /// <remarks/>
        public string CtlgCode
        {
            get
            {
                return ctlgCodeField;
            }
            set
            {
                ctlgCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ClientIIN
        {
            get
            {
                return clientIINField;
            }
            set
            {
                clientIINField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string OrganizationtIIN
        {
            get
            {
                return organizationtIINField;
            }
            set
            {
                organizationtIINField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string BankBIК
        {
            get
            {
                return bankBIКField;
            }
            set
            {
                bankBIКField = value;
            }
        }

        /// <remarks/>
        public string BankCode
        {
            get
            {
                return bankCodeField;
            }
            set
            {
                bankCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string AccountNumber
        {
            get
            {
                return accountNumberField;
            }
            set
            {
                accountNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string AccountType
        {
            get
            {
                return accountTypeField;
            }
            set
            {
                accountTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> OpeningDate
        {
            get
            {
                return openingDateField;
            }
            set
            {
                openingDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> ClosingDate
        {
            get
            {
                return closingDateField;
            }
            set
            {
                closingDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Сurrency
        {
            get
            {
                return сurrencyField;
            }
            set
            {
                сurrencyField = value;
            }
        }
    }

#endregion

#region Классы для работы с отчетами

    /// <summary>
    /// Описывает поля по кторым происходит отбор в отчете по дебиторской задолженности контрагента или отбор по акту сверки взаиморасчетов с контрагентом.
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class RRSelection
    {

        private System.Nullable<System.DateTime> startDateField;
        private System.Nullable<System.DateTime> endDateField;
        private string clientIINField;
        private string contractNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> StartDate
        {
            get
            {
                return startDateField;
            }
            set
            {
                startDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> EndDate
        {
            get
            {
                return endDateField;
            }
            set
            {
                endDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ClientIIN
        {
            get
            {
                return clientIINField;
            }
            set
            {
                clientIINField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ContractName
        {
            get
            {
                return contractNameField;
            }
            set
            {
                contractNameField = value;
            }
        }
    }

    /// <summary>
    /// Представляет строку отчета по дебиторской задолженности контрагента или строку акта сверки взаиморасчетов с контрагентом.
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.sample-package.org")]
    public partial class ReconciliationReportRow
    {

        private string clientField;
        private string contractField;
        private string docDateField;
        private string docNameField;
        private System.Nullable<float> debitField;
        private System.Nullable<float> creditField;
        private string сurrencyField;

        /// <remarks/>
        public string Client
        {
            get
            {
                return clientField;
            }
            set
            {
                clientField = value;
            }
        }

        /// <remarks/>
        public string Contract
        {
            get
            {
                return contractField;
            }
            set
            {
                contractField = value;
            }
        }

        /// <remarks/>
        public string DocDate
        {
            get
            {
                return docDateField;
            }
            set
            {
                docDateField = value;
            }
        }

        /// <remarks/>
        public string DocName
        {
            get
            {
                return docNameField;
            }
            set
            {
                docNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Debit
        {
            get
            {
                return debitField;
            }
            set
            {
                debitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Credit
        {
            get
            {
                return creditField;
            }
            set
            {
                creditField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Сurrency
        {
            get
            {
                return сurrencyField;
            }
            set
            {
                сurrencyField = value;
            }
        }
    }

#endregion
    
}

#pragma warning restore 1591