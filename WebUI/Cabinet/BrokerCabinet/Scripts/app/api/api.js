define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("app.api.Api", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("altaApi", ['$resource', '$window', function ($resource, $window) {
        var xlsxContentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            docxContentType = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
        var api = {
            url: $window.globalSettings.urlToApp,
            company: null,
            update: update,
        };

        function update() {
            api.supplier = $resource(api.url + "/api/supplier/:supplierId", { supplierId: '@supplierId' });
            api.company = $resource(api.url + "/api/company/:companyId/:companyCtrl", { companyId: '@companyId', companyCtrl: '@companyCtrl' },
                {
                    getBrokers: {
                        method: 'GET',
                        isArray: true,
                        params: {
                            companyCtrl: 'broker'
                        }
                    },
                    getReconciliation: {
                        method: "GET",
                        isArray: true,
                        url: api.url + "/api/company/reconciliation/:brokerId",
                    },
                    saveDoc: {
                        method: 'POST',
                        params: {
                            companyCtrl: 'docs'
                        },
                        url: api.url + "/api/company/document",
                        transformRequest: angular.identity,
                        headers: {
                            'Content-Type': undefined
                        }
                    }
                }
            );

            api.auction = $resource(api.url + "/api/auction/:auctionId/:auctionCtrl", { auctionId: '@auctionId', auctionCtrl: '@auctionCtrl' },
                {
                    getApplicants: {
                        method: "GET",
                        isArray: true,
                        params: {
                            auctionCtrl: 'applicants'
                        }
                    },
                    getSupplierOrder: {
                        method: "GET",
                        url: api.url + "/api/auction/:auctionId/supplier-orders/:supplierId"
                    },
                    createProcuratory: {
                        method: "POST",
                        params: {
                            auctionCtrl: "procuratory"
                        },
                        transformRequest: function (data) {
                            var request = new FormData();
                            request.append("form", angular.toJson(data.form));
                            request.append("file", data.file);
                            return request;
                        },
                        headers: { 'Content-Type': undefined }
                    },
                    updateTechSpec: {
                        method: "POST",
                        traditional: true,
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded"
                        },
                        hasBody: true,
                        url: api.url + "/api/auction/:auctionId/supplier/:supplierId/tech-spec/:lotId",
                        params: { auctionId: '@auctionId', supplierId: '@supplierId', lotId: "@lotId" }
                    },
                    updateTechSpecUseTemplate: {
                        method: "POST",
                        url: api.url + "/api/auction/:auctionId/supplier/:supplierId/tech-spec-use-template/:lotId",
                        params: { auctionId: '@auctionId', supplierId: '@supplierId', lotId: "@lotId" },
                        transformRequest: function (data) {
                            var request = new FormData();
                            request.append("form", angular.toJson(data.form));
                            request.append("file", data.file);
                            return request;
                        },
                        headers: { 'Content-Type': undefined }
                    },
                    getFinalReports: {
                        method: "GET",
                        url: api.url + "/api/auction/:auctionId/final-reports/:supplierId",
                        isArray: true
                    }
                }
            );

            api.order = $resource(api.url + "/api/order/:orderId", { orderId: '@orderId' }, {
                getOrders: {
                    method: "GET",
                    isArray: true
                }
            });

            api.archive = $resource(api.url + "/api/archive/:section/:id", { section: '@section', id: '@id' },
                {
                    getFilesInfo: {
                        method: "GET",
                        params: {
                            section: "file"
                        },
                        isArray: true
                    },
                    removeDocumentInList: {
                        method: "DELETE",
                        headers: {
                            'Content-Type': "application/x-www-form-urlencoded",
                        },
                        transformRequest: angular.identity,
                        url: api.url + "/api/company/document/:documentId"
                    }
                }
            );
            
            //old
            api.documentFormation = $resource(api.url + "/api/document-formation/:method/:option", { method: '@method', option: '@option' }, {

                generateSupplierOrder: {
                    method: "GET",
                    params: {
                        method: "generate-supplier-order"
                    },
                    headers: {
                        accept: docxContentType
                    },
                    cache: false,
                    responseType: 'arraybuffer',
                    transformResponse: function (data, headers, status) {
                        return {
                            headers: headers(),
                            content: new Blob([data], {
                                type: docxContentType
                            }),
                        };

                    }
                },
                generateProcuratory: {
                    method: "POST",
                    params: {
                        method: "generate-procuratory"
                    },
                    headers: {
                        'Content-Type': "application/x-www-form-urlencoded",
                        accept: docxContentType
                    },
                    transformRequest: angular.identity,
                    cache: false,
                    responseType: 'arraybuffer',
                    transformResponse: function (data, headers, status) {
                        return {
                            headers: headers(),
                            content: new Blob([data], {
                                type: docxContentType
                            }),
                        };

                    }
                },
                generateTechSpec: {
                    method: "GET",
                    params: {
                        method: "generate-tech-spec"
                    },
                    headers: {
                        'Content-Type': undefined,
                        accept: xlsxContentType
                    },
                    transformRequest: angular.identity,
                    cache: false,
                    responseType: 'arraybuffer',
                    transformResponse: function (data, headers, status) {
                        return {
                            headers: headers(),
                            content: new Blob([data], {
                                type: xlsxContentType
                            }),
                        };

                    }
                }
            });

            api.notification = $resource(api.url + "/api/notification/supplier", {});
        };
        update();
        return api;

    }]);

    return app;
});