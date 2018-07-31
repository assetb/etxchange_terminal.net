define([
    "angular",
    "angular-resource"
], function (angular) {
    angular.module("app.services.CustomerApi", ["ngResource"])
        .config(function ($resourceProvider) {
            $resourceProvider.defaults.stripTrailingSlashes = false;
        }).factory("CatalogApi", ["$resource", "$window", function ($resource, $window) {
            var url = $window.globalSettings.urlToApp;
            var api = {
                getMarkets: {
                    method: "GET",
                    isArray: true,
                    url: url + "/api/market"
                },
            };

            return $resource(url + "/api/customer/", {}, api);
        }]).factory("CustomerApi", ["$resource", "$window", function ($resource, $window) {
            var url = $window.globalSettings.urlToApp;
            var api = {
                getAuctions: {
                    method: "GET",
                    isArray: true,
                    url: url + "/api/customer/auction"
                }
            };

            return $resource(url + "/api/customer/", {}, api);
        }]).factory("OnlineApi", ["$resource", "$window", function ($resource, $window) {
            var url = $window.globalSettings.urlToApp;
            var urlForVPS = "http://109.120.140.23/ServerApp";
            var api = {
                getPriceOffersFromEts: {
                    method: "GET",
                    params: {
                        section: "ets"
                    }
                }
            };

            return $resource(urlForVPS + "/api/online/:section", { section: '@section' }, api);
        }]).factory("SupplierApi", ["$resource", "$window", function ($resource, $window) {
            var url = $window.globalSettings.urlToApp;

            var api = {
                getCompany: {
                    method: "GET",
                    url: url + "/api/supplier/:supplierId/company"
                },
                getDocuments: {
                    method: "GET",
                    url: url + "/api/supplier/:supplierId/documnet/other",
                    isArray: true
                },
                getProducts: {
                    method: "GET",
                    url: url + "/api/supplier/:supplierId/product",
                    isArray: true
                },
            };
            return $resource(url + "/api/supplier/", {}, api);
        }]);

    return angular.module("app.services.CustomerApi");
});