define([
    'angular',
    'alta-http'
], function (angular) {
    var app = angular.module('app.services.Search', ['app.services.Http']);

    app.factory('TableSearch', ['altaHttp', function (altaHttp) {
        var scope = {
            $url: null,
            $isLoading: false,
            $countShowItems: 0,
            $countItems: 0,
            $currentPage: 0,
            $countPages: 0,
            $lastSearchText: null,
            params: {
                query: null,
                page: 1,
                countItems: 10,
            },
            rows: [],
            success: function (data, status, header, config) {
                console.info("altaSearch", "method success is not implemented");
            },
            error: function (data, status, header, config) {
                console.info("altaSearch", "method error is not implemented");
            },
            Search: function Search() {
                scope.$isLoading = true;
                scope.$lastSearchText = scope.params.query;


                scope.$countShowItems = 0;
                scope.$countItems = 0;
                scope.$currentPage = 0;
                scope.$countPages = 0;
                scope.rows = null;

                altaHttp.get(scope.$url, {
                    method: "GET",
                    params: scope.params,
                }, function (data, status, header, config) {
                    scope.$countShowItems = data.countShowItems;
                    scope.$countItems = data.countItems;
                    scope.$currentPage = data.currentPage;
                    scope.$countPages = data.countPages;
                    scope.success && scope.success(data.rows, status, header, config);
                    scope.rows = data.rows;
                    scope.$isLoading = false;
                }, function (data, status, header, config) {
                    scope.$countShowItems = 0;
                    scope.$countItems = 0;
                    scope.$currentPage = 0;
                    scope.$countPages = 0;
                    scope.rows = [];
                    scope.error && scope.error(data, status, header, config);
                    scope.$isLoading = false;
                });
            }
        };
        return scope;
    }])  
        .factory("ListSearch", ['$http', function ($http) {
            var scope = {
                $url: null,
                $isLoading: false,
                $countShowItems: 0,
                $countItems: 0,
                $currentPage: 0,
                $countPages: 0,
                $lastSearchText: null,
                params: {
                    query: null,
                    page: 1,
                    countItems: 10,
                },
                rows: [],
                success: function (data, status, header, config) {
                    console.info("SearchFactory", "method success is not implemented");
                },
                error: function (data, status, header, config) {
                    console.info("SearchFactory", "method error is not implemented");
                },
                $lastSendedMessage: 0,
                Search: Search
            };

            function Search() {
                scope.$isLoading = true;
                scope.$lastSendedMessage++;
                scope.$lastSearchText = scope.params.query;

                scope.$countShowItems = 0;
                scope.$countItems = 0;
                scope.$currentPage = 0;
                scope.$selectedCount = 10;
                scope.$countPages = 0;         
                scope.rows = null;

                $http({
                    url: scope.$url,
                    method: "GET",
                    params: scope.params,
                    messageId: scope.$lastSendedMessage
                }).then(function (data, status, header, config) {
                    //if (config.messageId == scope.$lastSendedMessage) {
                        scope.$countShowItems = data.countShowItems;
                        scope.$countItems = data.countItems;
                        scope.$currentPage = data.currentPage;
                        scope.$countPages = data.countPages;
                        scope.success && scope.success(data.rows, status, header, config);
                        scope.rows = data.data;
                        scope.$isLoading = false;
                    //}
                });                   
                  
            };

            return scope;
        }]);
    return app;
});