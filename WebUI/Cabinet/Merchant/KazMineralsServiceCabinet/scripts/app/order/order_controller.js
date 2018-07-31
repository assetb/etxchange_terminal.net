(function () {
    'use strict';

    angular
        .module('app')
        .controller('order_controller', order_controller);

    order_controller.$inject = ['$scope', '$http', '$filter'];

    function order_controller($scope, $http, $filter) {
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
            customerid: CUSTOMER_ID,
            shemes: [],
            filesSize: 0,
            maxFilesSize: 33224432
        };

        $scope.SumFileSize = function () {
            var order = $scope.model.order;
            var agreement = $scope.model.agreement;
            var orderOrigin = $scope.model.orderOrigin;

            var bytes = (order ? order.size : 0) + (agreement ? agreement.size : 0) + (orderOrigin ? orderOrigin.size : 0);

            for (var i = 0; i < $scope.model.shemes.length; i++) {
                bytes += $scope.model.shemes[i] ? $scope.model.shemes[i].size : 0;
            }

            $scope.model.filesSize = bytes;
        };

        $scope.ClearModel = function () {
            $("input[type='file']").val("");
            $scope.model.siteId = null;
            $scope.model.number = "ВЦМ " + $filter('date')(new Date(), "dd-MM/yy") + "-0";
            $scope.model.order = null;
            $scope.model.agreement = null;
            $scope.model.orderOrigin = null;
            $scope.model.shemes = [];
        };

        $scope.SubmitOrder = function () {
            $scope.message = null;
            if ($scope.OrderForm.$invalid || !$scope.model.order || !$scope.model.agreement) {
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

            //file type sheme
            data.append("count_shemes", $scope.model.shemes.length);
            for (var i = 0; i < $scope.model.shemes.length; i++) {
                data.append("Sheme"+i, $scope.model.shemes[i]);
            }

            $scope.isSubmiting = true;
            $http.post(
            baseUrl + "/api/order",
            data,
            {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
            .success(function (data, status, headers, config) {
                if (data == null) {
                    $scope.message = "Ошибка при сохранении заявки. Обратитесь к своему брокеру.";
                } else {
                    $scope.message = "Заявка успешно добавлена.";
                    $scope.ClearModel();
                }
                $scope.GetOrders();
                $scope.isSubmiting = false;
            }).error(function (data, status, headers, config) {
                console.error("SubmitOrder. Error responce:", status, data);
                $scope.message = "Возникла ошибка при отправке";
                $scope.isSubmiting = false;
            });
        };

        $scope.GetOrders = function () {
            $scope.orders = null;
            $http.get(baseUrl + "/api/order", { params: { customerId: CUSTOMER_ID, statusId: 1 } })
                .success(function (data) {
                    $scope.orders = data;
                }).error(function (data, status, headers, config) {
                    console.error("GetOrders. Error responce:", status, data);
                    $scope.orders = [];
                });
        };

        $scope.GetSites = function () {
            $http.get(baseUrl + "/api/auction/sites")
            .success(function (data) {
                $scope.sites = data;
            }).error(function (data, status, headers, config) {
                console.error("GetSites. Error responce:", status, data);
                $scope.sites = [];
            });
        };

        $scope.GetFiles = function (fileListId) {
            $scope.files = null;
            $http.get(baseUrl + "/api/archive/list/" + fileListId).success(function (data) {
                $scope.files = data;
            }).error(function (data, status, headers, config) {
                console.error("GetFiles. Error responce:", status, data);
                $scope.files = [];
            });
        };

        $scope.Init = function () {
            $scope.ClearModel();
            $scope.GetOrders();
            $scope.GetSites();
        };
    };
})();