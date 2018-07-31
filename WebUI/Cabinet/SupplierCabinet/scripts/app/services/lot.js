define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("app.services.Lot", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    var LotApi = function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;
        var api = { };

        return $resource(url + "/api/lot/:lotId", { lotId: '@lotId' }, api);
    };
    
    LotApi.$inject = ['$resource', '$window'];

    app.factory("LotApi", LotApi);

    return app;
});