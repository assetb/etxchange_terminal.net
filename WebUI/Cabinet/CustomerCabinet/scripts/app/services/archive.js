define([
    'angular',
    'angular-resource',
], function (angular) {
    var app = angular.module("app.services.Archive", ["ngResource"]);

    app.config(function ($resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;
    });

    var ArchiveApi = function ($resource, $window) {
        var url = $window.globalSettings.urlToApp;
        var CONTENT_TYPE = "application/octet-stream";

        function transformResponse() {
            return function (data, headers, status) {
                return {
                    headers: headers(),
                    content: new Blob([data], {
                        type: CONTENT_TYPE
                    }),
                };
            }
        };

        var api = {
            getFile: {
                method: "GET",
                params: {
                    method: "file"
                },
                headers: {
                    accept: CONTENT_TYPE
                },
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            },
            getFilesList: {
                method: "GET",
                params: {
                    method: "list"
                },
                headers: {
                    accept: CONTENT_TYPE
                },
                cache: false,
                responseType: 'arraybuffer',
                transformResponse: transformResponse(),
            }
        }

        return $resource(url + "/api/archive/:method/:option", { method: '@method', option: '@option' }, api);
    };

    var ArchiveService = function (ArchiveApi) {
        var SuccessFunct = function (responce, headers) {
            var url = URL.createObjectURL(responce.content);
            var a = $(document.createElement("a"))
            .attr("name", "")
            .attr("type", "hidden")
            .attr("style", "display: none")
            .attr("href", url)
            .attr("download", decodeURIComponent(headers('Content-Disposition').match("filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)")[1]));

            $("body").append(a);

            a.get(0).click();
        };

        var errorFunc = function () {
            console.warn("Error in archive service");
        }

        this.getFile = function (fileId, success, error) {
            ArchiveApi.getFile({ option: fileId }, function (responce, headers) {
                SuccessFunct(responce, headers);
                success && success(responce, error);
            }, function (responce) {
                if (error)
                    error(responce);
                else
                    errorFunc(responce);
            });
        };

        this.getFilesList = function (filesListId, success, error) {
            ArchiveApi.getFilesList({ option: filesListId}, function (responce, headers) {
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