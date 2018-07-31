define([
    'angular',
    'angular-resource'
], function (angular) {
    var nameService = "AuctionApi";

    var app = angular.module("alta.service." + nameService, ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory(nameService, ['$resource', '$window', function ($resource, $window) {
        api = {
            url: $window.globalSettings.urlToApp,
        };

        (function() {
            api.qualification = $resource(api.url + "/api/auction/:auctionId/qualifications", { auctionId: '@auctionId' },
                {}
            );

        })();
        return api;
    }]);

    return app;
});