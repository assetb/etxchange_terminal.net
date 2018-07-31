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

        var api = {
            url: $window.globalSettings.urlToApp,
            company: null,
            update: update,
        };

        function update() {
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
            );

            api.auction = $resource(api.url + "/api/auction/:auctionId/:auctionCtrl", { auctionId: '@auctionId', auctionCtrl: '@auctionCtrl'},
                {
                    getApplicants: {
                        method: "GET",
                        isArray: true,
                        params: {
                            auctionCtrl: 'applicants'
                        }
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
                    }
                }
            );

            api.report = $resource(api.url + "/api/report/:section", { section: '@section' }, {
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
            });
        };
        update();
        return api;

    }]);

    return app;
});