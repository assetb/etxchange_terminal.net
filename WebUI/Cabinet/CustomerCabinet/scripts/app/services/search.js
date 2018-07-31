define([
    'angular'
], function (angular) {
    var app = angular
        .module('app.services.Search', [])
        .factory("SearchFactory", ['$http', function ($http) {
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
                scope.$countPages = 0;
                scope.$isActive = 'true';
                scope.rows = null;

                $http({
                        url: scope.$url,
                        method: "GET",
                        params: scope.params,
                        messageId: scope.$lastSendedMessage
                    }).success(function (data, status, headers, config) {
                        if (config.messageId == scope.$lastSendedMessage) {
                            scope.$countShowItems = data.countShowItems;
                            scope.$countItems = data.countItems;
                            scope.$currentPage = data.currentPage;
                            scope.$countPages = data.countPages;
                            scope.$isActive = (headers()['x-is-active']);    
                            scope.success && scope.success(data.rows, status, headers, config);

                            scope.rows = data.rows;

                            scope.$isLoading = false;
                        }
                    })
                    .error(function (data, status, headers, config) {
                        if (config.messageId == scope.$lastSendedMessage) {
                            scope.$countShowItems = 0;
                            scope.$countItems = 0;
                            scope.$currentPage = 0;
                            scope.$countPages = 0;
                            scope.rows = [];

                            scope.error && scope.error(data, status, headers, config);
                            scope.$isLoading = false;
                        }
                    });
            };

            return scope;
        }])
        .factory("Search", ['$http', function ($http) {
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
                success: function (data, status, headers, config) {
                    console.info("SearchFactory", "method success is not implemented");
                },
                error: function (data, status, headers, config) {
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
                scope.$isActive = 'true';
                scope.rows = null;

                $http({
                        url: scope.$url,
                        method: "GET",
                        params: scope.params,
                        messageId: scope.$lastSendedMessage
                    }).success(function (data, status, headers, config) {
                        
                        if (config.messageId == scope.$lastSendedMessage) {
                            scope.$countShowItems = data.length;
                            scope.$countItems = (headers()['x-count-items']);   
                            scope.$isActive = (headers()['x-is-active']); 
                            
//                            scope.$currentPage = data.currentPage;
                            scope.$countPages = +(Math.ceil(scope.$countItems/scope.params.countItems));
                            scope.rows = data;
                            scope.success && scope.success(data, status, headers, config);
                            scope.$isLoading = false;
                        }
                    })
                    .error(function (data, status, headers, config) {
                        if (config.messageId == scope.$lastSendedMessage) {
                            scope.$countShowItems = 0;
                            scope.$countItems = 0;
                            scope.$currentPage = 0;
                            scope.$countPages = 0;
                            scope.rows = [];
                            scope.error && scope.error(data, status, headers, config);
                            scope.$isLoading = false;
                        }
                    });
            };

            return scope;
        }]);
    return app;
});