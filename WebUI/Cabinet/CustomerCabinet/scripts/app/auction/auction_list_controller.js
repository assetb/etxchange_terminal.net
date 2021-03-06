﻿(function () {
    'use strict';

    function AuctionParametersClass() {
        this.numberOrProduct = null;
        this.statusid = null;
        this.site = null;
        this.fromDate = new Date(currentDate.getTime() - (3 * 24 * 60 * 60 * 1000));
        this.toDate = new Date(currentDate.getTime() + (16 * 24 * 60 * 60 * 1000));
        this.site = null;
        this.winner = 1;
    };

    angular
        .module('app')
        .controller('auctionListController', auction_list_controller);

    auction_list_controller.$inject = ['$window', '$scope', 'http_factory'];

    function auction_list_controller($window, $scope, http) {
        $scope.$urlToArchive = baseUrl + '/api/archive';
        $scope.model = {
            MAX_FILE_LIST_UPDALODED: 10,
            docsType: [
                { id: 16, $selected: true },
                { id: 9, $selected: true },
                { id: 6, $selected: true },
            ]
        };
        $scope.searchParameters = Object.assign(new SearchParametersClass(), new AuctionParametersClass());
        $scope.searchParameters.page = 1;
        $scope.table = new TableClass();
        $scope.statuses = [];
        $scope.markets = [];
        $scope.isLoading = false;

        $scope.orderBy = function (columnName) {
            if ($scope.searchParameters.orderby == columnName)
                $scope.searchParameters.isdesc = !$scope.searchParameters.isdesc;
            else {
                $scope.searchParameters.orderby = columnName;
                $scope.searchParameters.isdesc = false;
            }
            $scope.update();
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

            $scope.saveSearchParameters();

            http.get(baseUrl + "/api/customer/auction",
                {
                    params: $scope.searchParameters
                },
                function (auctions, status, header, config) {
                    $scope.table.countShowItems = auctions.length;
                    $scope.table.countItems = parseInt((header()['x-count-elements']));
                    $scope.table.countPages = (Math.ceil($scope.table.countItems / $scope.searchParameters.countItems));
                    $scope.table.rows = auctions;
                    $scope.isLoading = false;
                }, function (responce, status) {
                    notification.alert("Ошибка(" +  status + ")", "Обтатитесь к брокеру");
                    $scope.isLoading = false;
                });
        };

        $scope.GetMarkets = function () {
            http.get(baseUrl + "/api/market", null,
                function (markets) { $scope.markets = markets },
                function () {
                    $scope.markets = [];
                }
            );
        };
        $scope.loadStatuses = function () {
            http.get(
                baseUrl + "/api/auction/statuses",
                null,
                function (statuses) {
                    $scope.statuses = statuses;
                },
                function (data) {
                    $scope.statuses = [];
                }
            );
        };

        $scope.isSelectedAuctions = function () {
            var selectedCount = 0;
            if ($scope.table.rows) {
                angular.forEach($scope.table.rows, function (x) {
                    return x.$selected == true && selectedCount++;
                });
            }

            if (!$scope.model.docsType.some(function (el) {
                return el.$selected;
            })) {
                return false;
            }
            return selectedCount > 0 && !(selectedCount > $scope.model.MAX_FILE_LIST_UPDALODED);
        };

        $scope.donwloadArchive = function () {
            var auctions = $scope.table.rows.filter(function (el) {
                return el.$selected;
            });
            if (auctions) {
                var filesLists = [];
                angular.forEach(auctions, function (auction) {
                    return auction["<FilesListId>k__BackingField"] && filesLists.push(auction["<FilesListId>k__BackingField"]);
                });

                var docsTypeList = [];
                angular.forEach(
                    $scope.model.docsType,
                    function (docType) {
                        return docType.$selected && docsTypeList.push(docType.id);
                    }
                );

                $window.open($scope.$urlToArchive + "/list/zip?" + $.param({ list: filesLists, docsType: docsTypeList }));
            }
        };

        $scope.loadReports = function (auction) {
            http.get(
                $scope.$urlToArchive + "/list/" + auction["<FilesListId>k__BackingField"], { params: { types: [16] } },
                function (data) {
                    auction.$reports = data;
                },
                function () {
                    auction.$reports = null;
                }
            );
        };

        $scope.GetSumLots = function (lots) {
            var summ = 0.0;
            angular.forEach(lots, function (lot) {
                summ += lot.Price * lot.Quantity;
            });
            return summ;
        };

        $scope.saveSearchParameters = function () {
            if ($scope.winner == 1) {
                $window.sessionStorage.activeAuctionSearch = angular.toJson($scope.searchParameters);
            } else if ($scope.winner == 2) {
                $window.sessionStorage.historyAuctionSearch = angular.toJson($scope.searchParameters);
            }
        }

        $scope.loadSearchParameters = function () {
            if ($scope.winner == 1) {
                if ($window.sessionStorage.activeAuctionSearch != undefined) {
                    $scope.searchParameters = angular.fromJson($window.sessionStorage.activeAuctionSearch);
                }
            } else if ($scope.winner == 2) {
                if ($window.sessionStorage.historyAuctionSearch != undefined) {
                    $scope.searchParameters = angular.fromJson($window.sessionStorage.historyAuctionSearch);
                }
            }
        }

        $scope.Init = function (winner) {
            $scope.winner = winner;
            $scope.searchParameters.winner = winner;
            $window.globalSettings.$title.text(winner == 2 ? "Перечень прошедших аукционов" : "Список активных аукционов");
            $scope.loadSearchParameters();
            $scope.GetMarkets();
            $scope.update();
        };
    }
})();
