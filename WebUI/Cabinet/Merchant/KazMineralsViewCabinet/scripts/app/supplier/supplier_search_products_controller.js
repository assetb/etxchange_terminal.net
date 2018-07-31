(function () {
    'use strict';

    angular
        .module('app', ['ngSanitize'])
        .controller('supplier_search_products_controller', supplier_search_products_controller);

    supplier_search_products_controller.$inject = ['$scope', '$http'];

    function supplier_search_products_controller($scope, $http) {
        $scope.lastMessage = 0;
        $scope.i = 0;

        var ParametersClass = function () {
            this.searchsupplier = null;
            this.searchproduct = null;
        };

        $scope.searchParameters = Object.assign(new SearchParametersClass(), new ParametersClass());
        $scope.searchParameters.countItems = 50;
        $scope.table = new TableClass();
        $scope.isLoading = false;
        $scope.lastSearchText = "";
        $scope.range = function (n) {
            return new Array(n);
        };

        $scope.search = function () {
            $scope.update(1);
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

        $scope.MatchText = function (string, findText) {
            if (findText) {
                var re = new RegExp(findText, "i");
                return string.match(re) != null;
            }
            return false;
        };
        $scope.LigtingText = function (string, findText) {
            if (findText) {
                var re = new RegExp(findText, "i");
                string = string.replace(re, '<span style="background-color: #e7e7e7;">$&</span>');
            }
            return string;
        };

        $scope.update = function (page) {
            $scope.lastMessage++;
            console.log("Create message(", $scope.lastMessage, ") using parameters form:", $scope.searchParameters);

            $scope.searchParameters.page = page;
            $scope.isLoading = true;
            $http({
                url: baseUrl + "/api/supplier/with-products",
                method: "GET",
                params: $scope.searchParameters,
                messageId: $scope.lastMessage
            }).success(function (data, status, header, config) {
                console.log("messageId:", config.messageId);
                if (config.messageId == $scope.lastMessage) {
                    console.log("is last message");
                    $scope.lastSearchText = $scope.searchParameters.searchproduct;
                    $scope.table = data;
                    $scope.isLoading = false;
                }
            })
                .error(function (data, status, headers, config) {
                    if (config.messageId == $scope.lastMessage) {
                        $scope.table = new TableClass();
                        $scope.isLoading = false;
                    }
                });
        };

        $scope.Init = function () {
            $scope.update(1);
        }
    }
})();
