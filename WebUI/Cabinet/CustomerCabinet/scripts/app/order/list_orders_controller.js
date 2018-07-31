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
            http.get(baseUrl + "/api/customer/order",
                { params: { statusId: 1 } },
                function (data) {
                  
                        $scope.orders = data;
                     
                },
                function (data, status, headers, config) {
                    notification.alert("Ошибка (" + status + ").", "Не удалось загрузить список заявок");
                    $scope.orders = [];
                });
        };

        $scope.GetFiles = function (fileListId) {
            $scope.files = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId,
                null,
                function (data) {
                    $scope.files = data;
                    //$('#filesModal').foundation('reveal', 'open');
                },
                function (data, status, headers, config) {
                    console.error("GetFiles. Error responce:", status, data);
                    $scope.files = [];
                });
        };
        $scope.toggle_dom = function (element, button) {
            $(element).slideToggle(function () {
                $(button).text(
                    $(this).is(':visible') ? "Скрыть" : "Показать"
                );
            });
        };

        $scope.Init = function () {
                $scope.Update();
        };
    }
})();
