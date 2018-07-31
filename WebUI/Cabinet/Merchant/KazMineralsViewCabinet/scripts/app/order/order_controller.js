(function () {
    'use strict';

    angular
        .module('app')
        .controller('order_controller', order_controller);

    order_controller.$inject = ['$scope', '$http', 'order_upload_service'];

    function order_controller($scope, $http, order_upload_service) {
        $scope.title = 'order_controller';
        $scope.orders = null;
        $scope.sites = null;
        $scope.files = null;
        $scope.isSubmiting = false;
        $scope.URL_ARCHIVE = baseUrl + '/api/archive/file';
        $scope.model = {
            siteId: null,
            number: null,
            order: null,
            agreement: null,
            orderOrigin: null,
            customerid: CUSTOMER_ID
        }

        $scope.SubmitOrder = function () {
            $scope.isSubmiting = true;
            $scope.message = null;
            if ($scope.OrderForm.$invalid || !$scope.model.order || !$scope.model.orderOrigin || !$scope.model.agreement) {
                $scope.message = "Заполните все поля";
                return;
            }

            var data = new FormData();
            data.append("siteId", $scope.model.siteId);
            data.append("number", $scope.model.number);
            data.append("customerid", $scope.model.customerid);

            // files
            data.append("Order", $scope.model.order);
            data.append("Agreement", $scope.model.agreement);
            data.append("OrderOrigin", $scope.model.orderOrigin);


            $http.post(
            baseUrl + "/api/order",
            data,
            {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
            .success(function (data) {
                if (data == null) {
                    $scope.message = "Ошибка при сохранении заявки. Обратитесь к своему брокеру.";
                } else {
                    $scope.message = "Заявка успешно добавлена.";
                    $scope.model = null;
                }
                $scope.GetOrders();
                $scope.isSubmiting = false;
            }).error(function (data) {
                console.error("Ошибка сохранения заявки");
                $scope.isSubmiting = false;
            });
        };

        $scope.GetOrders = function () {
            $scope.orders = null;
            $http.get(baseUrl + "/api/order", { params: { customerId: CUSTOMER_ID, statusId:1 } }).success(function (data) {
                $scope.orders = data;
            });
        };

        $scope.GetSites = function () {
            $http.get(baseUrl + "/api/auction/sites")
            .success(function (data) {
                $scope.sites = data;
            });
        };


        $scope.GetFiles = function (fileListId) {
            $scope.files = null;
            $http.get(baseUrl + "/api/archive/list/" + fileListId).success(function (data) {
                $scope.files = data;
            });
        };

        $scope.Init = function () {
            $scope.GetOrders();
            $scope.GetSites();
        };
    };
})();