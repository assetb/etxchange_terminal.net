(function () {
    'use strict';

    function AuctionParametersClass() {
        this.customerid = null;
        this.numberOrProduct = null;
        this.statusid = null;
        this.site = null;
        this.fromDate = new Date(currentDate.getTime() - (3 * 24 * 60 * 60 * 1000));
        this.toDate = new Date(currentDate.getTime() + (16 * 24 * 60 * 60 * 1000));
        this.site = null;
        this.winner = 1;
        this.customerId = CUSTOMER_ID;
    };

    angular
        .module('app')
        .controller('auctionListController', auction_list_controller);

    auction_list_controller.$inject = ['$window', '$scope', 'http_factory'];

    function auction_list_controller($window, $scope, http) {
        $scope.$urlToArchive = baseUrl + '/api/archive';

        $scope.searchParameters = Object.assign(new SearchParametersClass(), new AuctionParametersClass());
        $scope.searchParameters.page = 1;
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

        $scope.updateTable = function (page, items) {
            $scope.searchParameters.countItems = items;
            $scope.update(page)
        };

        $scope.update = function (page) {
            if (page)
                $scope.searchParameters.page = page;
            $scope.isLoading = true;

            http.get(baseUrl + "/api/auction",
                {
                    params: $scope.searchParameters
                },
                function (data, status, header, config) {
                    $scope.table = data;
                    $scope.isLoading = false;
                },
                function (data, status, headers, config) {
                    $scope.table = [];
                    $scope.isLoading = false;
                });
        };

        $scope.GetMarkets = function () {
            http.get(baseUrl + "/api/market", null,
                function (data) {
                    $scope.markets = data;
                },
                function (data, status, headers, config) {
                    console.error("GetMarkets. Error responce:", status, data);
                    $scope.markets = [];
                });
        };
        $scope.loadStatuses = function () {
            http.get(baseUrl + "/api/auction/statuses", null,
                function (data, status, header, config) {
                    $scope.statuses = data;
                },
                function (data, status, headers, config) {
                    $scope.statuses = [];
                });
        };



        $scope.loadReports = function (auction) {
            http.get($scope.$urlToArchive + "/list/" + auction["<FilesListId>k__BackingField"], { params: { types: [16] } },
                function (data) {
                    auction.$reports = data;
                },
                function (data, status, headers, config) {
                    auction.$reports = null;
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
            $window.globalSettings.$title.text(winner == 2 ? "Перечень прошедших аукционов" : "Список активных аукционов");
            $scope.GetMarkets();
            $scope.update();
        };
    }
})();
