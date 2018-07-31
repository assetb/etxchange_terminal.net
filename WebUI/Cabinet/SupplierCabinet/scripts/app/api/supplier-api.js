define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("alta.service.SupplierApi", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("SupplierApi", ['$resource', '$window', function ($resource, $window) {
        var xlsxContentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            docxContentType = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
        var api = {
            url: $window.globalSettings.urlToApp,
            company: null,
            update: update,
        };

        function MultipartTransformRequest(data) {
            var request = new FormData();
            request.append("form", angular.toJson(data.form));
            angular.forEach(data.files, function (fileData, fileName) {
                request.append(fileName, fileData);
            });
            return request;
        };

        function update() {
            api.supplier = $resource(api.url + "/api/supplier", {},
                {

                    // Auction
                    getAuctions: {
                        method: "GET",
                        url: api.url + "/api/supplier/auction",
                    },

                    // Company
                    getCompany: {
                        method: 'GET',
                        url: api.url + "/api/supplier/company",
                    },
                    updateCompany: {
                        method: 'POST',
                        url: api.url + "/api/supplier/company",
                    },

                    // Product
                    getProducts: {
                        method: 'GET',
                        url: api.url + "/api/supplier/product",
                        isArray: true
                    },
                    deleteProduct: {
                        method: 'DELETE',
                        url: api.url + "/api/supplier/product/:productId",
                    },
                    createProduct: {
                        method: 'POST',
                        url: api.url + "/api/supplier/product",
                        headers: {
                            "Content-Type": undefined
                        },
                        transformRequest: function (data) {
                            return data;
                        },
                    },

                    // Document
                    getAuctionDocuments: {
                        method: "GET",
                        isArray: true,
                        url: api.url + "/api/supplier/auction/:auctionId/document"
                    },
                    getContractsDocuments: {
                        method: 'GET',
                        url: api.url + "/api/supplier/documnet/contract",
                        isArray: true
                    },
                    getOtherDocuments: {
                        method: 'GET',
                        url: api.url + "/api/supplier/documnet/other",
                        isArray: true
                    },

                    // SupplierOrder
                    getSupplierOrder: {
                        method: 'GET',
                        url: api.url + "/api/supplier/auction/:auctionId/order",
                    },
                    putSupplierOrder: {
                        method: "POST",
                        url: api.url + "/api/supplier/auction/:auctionId/order/",
                        transformRequest: MultipartTransformRequest,
                        headers: { 'Content-Type': undefined }
                    },

                    // Final report
                    getFinalReports: {
                        method: 'GET',
                        isArray: true,
                        url: api.url + "/api/supplier/auction/:auctionId/final-report",
                    },

                    // Procuratory
                    putProcuratory: {
                        method: "POST",
                        url: api.url + "/api/supplier/auction/:auctionId/procuratory",
                        transformRequest: MultipartTransformRequest,
                        headers: { 'Content-Type': undefined }
                    },

                    // TechSpec
                    updateTechSpec: {
                        method: "POST",
                        traditional: true,
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded"
                        },
                        hasBody: true,
                        url: api.url + "/api/supplier/auction/:auctionId/lot/:lotId/tech-spec",
                    },
                    updateTechSpecUseTemplate: {
                        method: "POST",
                        url: api.url + "/api/supplier/auction/:auctionId/lot/:lotId/tech-spec-use-template",
                        transformRequest: function (data) {
                            var request = new FormData();
                            request.append("form", angular.toJson(data.form));
                            request.append("file", data.file);
                            return request;
                        },
                        headers: { 'Content-Type': undefined }
                    },
                }
            );

        };
        update();
        return api;

    }]);

    return app;
});