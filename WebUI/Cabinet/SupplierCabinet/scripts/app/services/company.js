define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("app.services.Company", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    var CompanyApi = function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;

        var api = {
            save: {
                method: "POST",
                transformRequest: angular.identity,
                headers: {
                    "Content-Type": "application/json",
                },
            }
        };

        return $resource(url + "/api/company/:companyId", { companyId: '@companyId' }, api);
    };

    CompanyApi.$inject = ['$resource', '$window'];

    app.factory("CompanyApi", CompanyApi);

    return app;
});