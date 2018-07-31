(function () {
    'use strict';

    angular
        .module('app')
        .controller('list_orders_controller', order_list_controller);

    order_list_controller.$inject = ['$scope', 'http_factory', '$route'];

    function order_list_controller($scope, http, $route) {
        $scope.$route = $route;
        $scope.orders = [];
        $scope.files = null;
        $scope.URL_ARCHIVE = baseUrl + '/api/archive/file';

        $scope.Update = function () {
            $scope.orders = null;
            http.get(baseUrl + "/api/order",
                { params: { customerId: CUSTOMER_ID, statusId: 1 } },
                function (data) {
                    $scope.orders = data;
                },
                function (data, status, headers, config) {
                    console.error("GetOrders. Error responce:", status, data);
                    $scope.orders = null;
                });
        };

        $scope.GetFiles = function (fileListId) {
            $scope.files = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId,
                null,
                function (data) {
                    $scope.files = data;
                    $('#filesModal').foundation('reveal', 'open');
                },
                function (data, status, headers, config) {
                    console.error("GetFiles. Error responce:", status, data);
                    $scope.files = [];
                });
        };

        $scope.Init = function () {
            $scope.Update();
        };
    }
})();
