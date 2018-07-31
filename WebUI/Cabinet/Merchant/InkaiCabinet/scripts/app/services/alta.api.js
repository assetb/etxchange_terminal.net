/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular/angular.js" />
define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("alta.service.api", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("altaApi", ['$resource', '$window', function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;
        var urlForVPS = "http://109.120.140.23/ServerApp";
        var api = {
            company: $resource(url + "/api/company/:companyId/:companyCtrl", { companyId: '@companyId', companyCtrl: '@companyCtrl' },
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
                        params: {
                            companyCtrl: 'reconciliation'
                        }
                    },
                    saveDoc: {
                        method: 'POST',
                        params: {
                            companyCtrl: 'docs'
                        },
                        transformRequest: angular.identity,
                        headers: {
                            'Content-Type': undefined
                        }
                    }
                }
            ),
            auction : $resource(url + "/api/auction/:auctionId/:auctionCtrl", { auctionId: '@auctionId', auctionCtrl: '@auctionCtrl'},
                {
                    getApplicants: {
                        method: "GET",
                        isArray: true,
                        params: {
                            auctionCtrl: 'applicants'
                        }
                    }
                }
            ),
            order : $resource(url + "/api/order/:orderId", { orderId: '@orderId' }, {
                getOrders: {
                    method: "GET",
                    isArray: true
                }
            }),
            archive : $resource(url + "/api/archive/:section/:id", { section: '@section', id: '@id' }, {
                getFilesInfo: {
                    method: "GET",
                    params: {
                        section: "file"
                    },
                    isArray: true
                }
            }),
            report: $resource(url + "/api/report/:section", { section: '@section' }, {
                getTechSpec: {
                    method: "GET",
                    params: {
                        section: "tech_spec"
                    }
                },
                generateTechSpec: {
                    method: "GET",
                    params: {
                        section: "tech_spec/generate"
                    },
                    isArray:true
                }
            }),
            online: $resource(urlForVPS + "/api/online/:section", { section: '@section' }, {
                ets: {
                    method: "GET",
                    params: {
                        section: "ets"
                    }
                }
            }),
            market: $resource(url + "/api/market")
    };
    return api;

}]);

return app;
});