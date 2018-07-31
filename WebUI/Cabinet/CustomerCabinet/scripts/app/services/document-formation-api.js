define([
    'angular',
    'angular-resource'
], function (angular) {
    var app = angular.module("alta.service.DocumentFormationApi", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("documentFormationApi", ['$resource', '$window', function ($resource, $window) {
        var xlsxContentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            docxContentType = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';

        var api = {
            url: $window.globalSettings.urlToApp,
            company: null,
            update: update,
        };

        function transformResponse(docType) {
            return function (data, headers, status) {
                return {
                    headers: headers(),
                    content: new Blob([data], {
                        type: docType
                    }),
                };
            }
        };

        function update() {
            api.documentFormation = $resource(api.url + "/api/document-formation/:method/:option", { method: '@method', option: '@option' }, {
                reportTechSpec: {
                    method: "GET",
                    params: {
                        method: "report-tech-spec"
                    },
                    headers: {
                        accept: docxContentType
                    },
                    cache: false,
                    responseType: 'arraybuffer',
                    transformResponse: transformResponse(xlsxContentType),
                },
            });
        };
        update();
        return api;
    }]);

    return app;
});