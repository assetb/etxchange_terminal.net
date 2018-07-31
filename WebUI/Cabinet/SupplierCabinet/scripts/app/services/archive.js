define([
    'angular',
    'angular-resource',
    'notification'
], function (angular) {
    var app = angular.module("app.services.Archive", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    var ArchiveApi = function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;

        function transformResponse() {
            return function (data, headers, status) {
                return {
                    headers: headers(),
                    content: new Blob([data], {
                        type: "application/octet-stream"
                    }),
                };
            }
        };

        var api = {
            getFile: {
                method: "GET",
                url: url + "/api/archive/file/:fileId",
                headers: {
                    accept: "application/octet-stream"
                },
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            }
        }

        return $resource(url + "/api/document-formation/:method/:option", { method: '@method', option: '@option' }, api);
    };

    var ArchiveService = function (ArchiveApi) {
        var SuccessFunct = function (responce, headers) {
            var url = URL.createObjectURL(responce.content);
            var a = document.createElement("a");
            a.name = "";
            a.style = "display: none";
            a.href = url;
            a.download = decodeURIComponent(headers('Content-Disposition').match("filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)")[1]);
            a.click();
        };

        var errorFunc = function () {
            notification.showErrApplication();
        }

        this.getFile = function (fileId, success, error) {
            ArchiveApi.getFile({ fileId: fileId }, function (responce, headers) {
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

    ArchiveApi.$inject = ['$resource', '$window'];
    ArchiveService.$inject = ["ArchiveApi"];

    app.factory("ArchiveApi", ArchiveApi);
    app.factory("ArchiveService", ArchiveService);

    return app;
});