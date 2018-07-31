define([
    'angular',
    'angular-resource'
], function (ng) {
    var app = ng.module("alta.api.altatender", ["ngResource"]);
    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    app.factory("AltatenderApi", ['$resource', '$window', function ($resource, $window) {
        var url = "http://altatender.kz:8080/protocolREST";
        var TIMEOUT = 30000;
        var api = {
            protocols: $resource(url + "/protocols/:section", { section: "@section" }, {
                count: {
                    timeout: TIMEOUT,
                    method: "GET",
                    params: {
                        section: "count"
                    }
                }
            }),
            lots: $resource(url + "/protocols/lots/:section", { section: "@section" }, {
                count: {
                    timeout: TIMEOUT,
                    method: "GET",
                    params: {
                        section: "count"
                    }
                },
                find: {
                    timeout: TIMEOUT,
                    method: "GET",
                    params: {
                        section: "find"
                    }
                }
            }),
            winCount: $resource(url + "/protocols/lots/wincount/:section", {section: "@section" }, {
                count: {
                    timeout : TIMEOUT,
                    method: "GET",
                    params: {
                        section: "count"
                    }
                }
            })
        };
        return api;

    }]);

    return app;
});