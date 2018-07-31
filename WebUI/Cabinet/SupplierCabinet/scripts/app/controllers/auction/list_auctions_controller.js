define([
    'angular',
    'alta-http',
    //"app.directives.Table"
], function (angular) {
    var app = angular.module('alta.auction.list', ['alta.service.http']);
    var SHOW_ACTIVE = 1, SHOW_FINISHED = 2, DEFAULT_PAGE = 1, DEFAULT_COUNT_ITEMS = 10;

    app.controller("auctionListController", ['$scope', '$window', 'altaHttp', function ($scope, $window, http) {
        $scope.$$lastMessageId = 0;
        $scope.searchParameters = {
            page: DEFAULT_PAGE,
            countItems: DEFAULT_COUNT_ITEMS,
            numberOrProduct: null,
            statusid: null,
            site: null,
            fromDate: new Date((new Date()).getTime() - (7 * 24 * 60 * 60 * 1000)),
            toDate: new Date((new Date()).getTime() + (7 * 24 * 60 * 60 * 1000)),
            site: null,
            all: false,
            winner: SHOW_ACTIVE
        };
        $scope.table = {};

        $scope.statuses = [];
        $scope.markets = [];
        $scope.customers = [];
        $scope.isLoading = false;

        $scope.range = function (n) {
            return new Array(n);
        };

        $scope.updateTable = function (page, items) {
            $scope.searchParameters.countItems = items;
            $scope.update(page);
        };
        $scope.search = function () {
            $scope.update(DEFAULT_PAGE);
        };

        $scope.update = function (page) {
            $scope.searchParameters.page = page;
            $scope.isLoading = true;
            var id = ++$scope.$$lastMessageId;
            http.get(baseUrl + "/api/supplier/auction",
                {
                    params: $scope.searchParameters
                },
                function (data, status, header, config) {
                    $scope.table = data;
                    $scope.isLoading = false;
                }, function (data, status, headers, config) {
                    $scope.table = null;
                    $scope.isLoading = false;
                    notification.showWarn("Произошла ошибка при загрузке списка аукционов. Повторите попытку позже или обратитесь к брокеру.");
                });
        };

        $scope.GetPagesForPagination = function () {
            var arrayPages = [];
            var COUNT_PAGINATION_ITEMS = 7;
            var current = $scope.table.currentPage;
            var count = $scope.table.countPages;

            if (current && count) {
                for (var i = 0; i < COUNT_PAGINATION_ITEMS; i++) {
                    var page;
                    if (current <= Math.floor(COUNT_PAGINATION_ITEMS / 2)) {
                        page = 1;
                    } else {
                        if (current >= (count - Math.floor(COUNT_PAGINATION_ITEMS / 2))) {
                            page = (count - COUNT_PAGINATION_ITEMS + 1);
                        } else {
                            page = current - Math.floor(COUNT_PAGINATION_ITEMS / 2);
                        }
                    }
                    if (page <= 0) page = 1;
                    page = page + i;
                    if (page > count) {
                        break;
                    }
                    arrayPages.push(page);
                }
            }
            return arrayPages;
        };

        $scope.GetMarkets = function () {
            http.get(baseUrl + "/api/market", null, function (data) {
                $scope.markets = data;
            }, function () {
                notification.showWarn("Произошла ошибка при загрузке списка товарных площадок. Повторите попытку позже или обратитесь к брокеру.");
            });
        };

        $scope.GetCustomers = function () {
            http.get(baseUrl + "/api/customer", {
                params: {
                    page: 1, countItems: 10
                }
            }, function (data) {
                $scope.customers = data.rows;
            }, function () {
                notification.showWarn("Произошла ошибка при загрузке списка заказчиков. Повторите попытку позже или обратитесь к брокеру.");
            });
        };

        $scope.loadStatuses = function () {
            http.get(baseUrl + "/api/auction/statuses", null, function (data, status, header, config) {
                $scope.statuses = data;
            }, function (data, status, headers, config) {
                $scope.statuses = [];
            }, function () {
                notification.showWarn("Произошла ошибка при загрузке списка статусов. Повторите попытку позже или обратитесь к брокеру.");
            });
        };

        $scope.GetSumLots = function (lots) {
            var summ = 0.0;
            for (var i in lots) {
                summ += lots[i].Price * lots[i].Quantity;
            }
            return summ;
        };

        $scope.Init = function (winner) {
            $scope.searchParameters.winner = winner;
            $scope.loadStatuses();
            $scope.GetMarkets();
            $scope.GetCustomers();
            $scope.update(DEFAULT_PAGE);
        };
    }]);

    return app;
});
