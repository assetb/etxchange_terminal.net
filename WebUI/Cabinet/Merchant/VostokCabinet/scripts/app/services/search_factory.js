(function () {
    'use strict';
    var SERVICE_NAME = 'search_factory';
    angular
        .module('app')
        .factory(SERVICE_NAME, ['$http', function ($http) {
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
                    console.info(SERVICE_NAME, "method success is not implemented");
                },
                error: function (data, status, header, config) {
                    console.info(SERVICE_NAME, "method error is not implemented");
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
                scope.rows = null;

                $http({
                    url: scope.$url,
                    method: "GET",
                    params: scope.params,
                    messageId: scope.$lastSendedMessage
                }).success(function (data, status, header, config) {
                    if (config.messageId == scope.$lastSendedMessage) {
                        scope.$countShowItems = data.countShowItems;
                        scope.$countItems = data.countItems;
                        scope.$currentPage = data.currentPage;
                        scope.$countPages = data.countPages;

                        scope.success && scope.success(data.rows, status, header, config);

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
        }]);
})();