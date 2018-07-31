define([
    'angular',
    'angular-resource',
    'notification'
], function (angular) {
    var app = angular.module("app.services.DocumentFormation", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    var DocumentFormationApi = function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;
        var CONTENT_TYPE = "application/octet-stream";

        function transformResponse() {
            return function (data, headers, status) {
                return {
                    headers: headers(),
                    content: new Blob([data], {
                        type: CONTENT_TYPE
                    }),
                };
            }
        };

        var api = {
            reportTechSpec: {
                method: "GET",
                params: {
                    method: "report-tech-spec"
                },
                headers: {
                    accept: CONTENT_TYPE
                },
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            },
            generateSupplierOrder: {
                method: "GET",
                params: {
                    method: "generate-supplier-order"
                },
                headers: {
                    accept: CONTENT_TYPE
                },
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            },
            generateProcuratory: {
                method: "POST",
                url: url + "/api/document-formation/auction/:auctionId/generate-procuratory",
                headers: {
                    'Content-Type': "application/x-www-form-urlencoded",
                    accept: CONTENT_TYPE
                },
                transformRequest: angular.identity,
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse()
            },
            generateTechSpec: {
                method: "GET",
                params: {
                    method: "generate-tech-spec"
                },
                headers: {
                    'Content-Type': undefined,
                    accept: CONTENT_TYPE
                },
                transformRequest: angular.identity,
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            }
        }

        return $resource(url + "/api/document-formation/:method/:option", { method: '@method', option: '@option' }, api);
    };

    var DocumentFormationService = function (DocumentFormationApi) {
        var SuccessFunct = function (responce, headers) {
            var url = URL.createObjectURL(responce.content);
            var a = $(document.createElement("a"))
            .attr("name", "")
            .attr("type", "hidden")
            .attr("style", "display: none")
            .attr("href", url)
            .attr("download", decodeURIComponent(headers('Content-Disposition').match("filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)")[1]));

            $("body").append(a);
            
            a.get(0).click();
        };

        var errorFunc = function () {
            notification.showErrApplication();
        }

        this.generateTechSpec = function (auctionId, lotId, success, error) {
            DocumentFormationApi.generateTechSpec({ auctionId: auctionId, lotId: lotId }, function (responce, headers) {
                SuccessFunct(responce, headers);
                success && success(responce, error);
            }, function (responce) {
                if (error)
                    error(responce);
                else
                    errorFunc(responce);
            });
        };

        this.generateProcuratory = function (auctionId, autoCouting, lots, success, error) {
            DocumentFormationApi.generateProcuratory({ auctionId: auctionId, autoCouting: autoCouting }, $.param({ lots: lots }), function (responce, headers) {
                SuccessFunct(responce, headers);
                success && success(responce, error);
            }, function (responce) {
                if (error)
                    error(responce);
                else
                    errorFunc(responce);
            });
        };

        this.generateSupplierOrder = function (auctionId, lots, success, error) {
            DocumentFormationApi.generateSupplierOrder({ auctionId: auctionId, lots: lots }, function (responce, headers) {
                SuccessFunct(responce, headers);
                success && success(responce, error);
            }, function (responce) {
                if (error)
                    error(responce);
                else
                    errorFunc(responce);
            });
        };

        return this;
    };

    DocumentFormationApi.$inject = ['$resource', '$window'];
    DocumentFormationService.$inject = ["DocumentFormationApi"];

    app.factory("DocumentFormationApi", DocumentFormationApi);
    app.factory("DocumentFormationService", DocumentFormationService);

    return app;
});