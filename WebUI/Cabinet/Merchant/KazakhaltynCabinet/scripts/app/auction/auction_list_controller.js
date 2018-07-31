(function () {
    'use strict';

    function AuctionParametersClass() {
        this.customerid = null;
        this.numberOrProduct = null;
        this.statusid = null;
        this.site = null;
        this.fromDate = new Date(currentDate.getTime() - (7 * 24 * 60 * 60 * 1000));
        this.toDate = new Date(currentDate.getTime() + (7 * 24 * 60 * 60 * 1000));
        this.site = null;
        this.winner = 1;
        this.customerId = CUSTOMER_ID;
    };

    angular
        .module('app')
        .controller('auctionListController', auction_list_controller);

    auction_list_controller.$inject = ['$scope', '$http'];

    function auction_list_controller($scope, $http) {
        $scope.searchParameters = Object.assign(new SearchParametersClass(), new AuctionParametersClass());
        $scope.table = new TableClass();

        $scope.statuses = [];
        $scope.markets = [];
        $scope.isLoading = false;

        $scope.range = function (n) {
            return new Array(n);
        };

        $scope.search = function () {
            $scope.update(1);
        };

        $scope.update = function (page) {
            console.log($scope.searchParameters);
            $scope.searchParameters.page = page;
            $scope.isLoading = true;
            $http({
                url: baseUrl + "/api/auction",
                method: "GET",
                params: $scope.searchParameters,
            }).success(function (data, status, header, config) {
                console.log("Succes GetAuction request:", data);
                $scope.table = data;
                $scope.isLoading = false;
            })
                .error(function (data, status, headers, config) {
                    $scope.table = null;
                    $scope.isLoading = false;
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
            $http.get(baseUrl + "/api/market")
            .success(function (data) {
                $scope.markets = data;
            });
        };
        $scope.loadStatuses = function () {
            $http({
                url: baseUrl + "/api/auction/statuses",
                method: "GET",
            }).success(function (data, status, header, config) {
                $scope.statuses = data;
            })
                .error(function (data, status, headers, config) {
                    $scope.statuses = [];
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
            $scope.update(1);
        };
    }
})();
