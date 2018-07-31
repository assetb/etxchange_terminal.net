/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular/angular.js" />
define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("altatender.service.api", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("AltatenderApiFactory", ['$resource', function ($resource) {
        return function (baseUrl) {
            //this.baseUrl = baseUrl;

			this.baseUrl = "http://altatender.kz:8080/AltaTender-1.0-SNAPSHOT.Samruk-branch";

            this.Search = $resource(this.baseUrl + "/rest/web/api/search");

            this.Catalog = $resource(this.baseUrl + "/rest/web/api/catalog/:section", { section: '@section' }, {
                getMethods: {
                    method: "GET",
                    params: { section: 'method' }
                },
                getSources: {
                    method: "GET",
                    params: { section: 'source' }
                },
                getRegions: {
                    method: "GET",
                    params: { section: 'region' }
                }
            });
        };
    }]);

    return app;
});