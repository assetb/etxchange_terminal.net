require([
    'angular',
    "supplier-api",
        "pagination"
], function (angular) {
    angular
        .module('app', [
            "alta.directive.pagination",
            "alta.service.SupplierApi"
        ])
        .controller('customer_profile_controller', customer_profile_controller);

    customer_profile_controller.$inject = ['$scope', '$http', "SupplierApi"];

    function customer_profile_controller($scope, $http, SupplierApi) {
        var DEFAULT_PAGE = 1;
        var DEFAULT_COUNT_ITEMS = 10;
        var SHOW_ACTIVE = 1;

        $scope.table;
        $scope.customerId = null;
        $scope.company = null;
        $scope.searchParameters = {
            page: DEFAULT_PAGE,
            countItems: DEFAULT_COUNT_ITEMS,
            numberOrProduct: null,
            statusid: null,
            site: null,
            all: true,
            winner: SHOW_ACTIVE
        }

        $scope.GetCompany = function () {
            $http.get(baseUrl + "/api/customer/" + $scope.customerId)
                .success(function (data) {
                    $scope.company = data;
                })
                .error(function (data) {
                    $scope.company = null;
                });
        };

        $scope.getAuctions = function () {
            $scope.searchParameters.customerid = $scope.customerId;
            $scope.table = null;
            SupplierApi.supplier.getAuctions($scope.searchParameters,
                function (table) {
                    $scope.table = table;
                }, function () {
                    notification.showErrApplication();
                });
        };

        $scope.updateTable = function (page, items) {
            $scope.searchParameters.countItems = items;
            $scope.getAuctions(page);
        };
        $scope.search = function () {
            $scope.getAuctions(DEFAULT_PAGE);
        };

        $scope.getAuctions = function (page) {
            $scope.searchParameters.page = page;
            $scope.searchParameters.customerid = $scope.customerId;
            $scope.isLoading = true;

            $scope.table = null;
            SupplierApi.supplier.getAuctions($scope.searchParameters,
                function (table) {
                    $scope.table = table;
                    $scope.isLoading = false;
                }, function () {
                    $scope.isLoading = false;
                    notification.showErrApplication();
                });
        };

        $scope.init = function (customerId) {
            $scope.customerId = customerId;
            $scope.GetCompany();
            $scope.getAuctions(DEFAULT_PAGE);
        }
    }

    angular.bootstrap($('#main_container'), ['app']);
});
