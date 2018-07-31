(function () {
    'use strict';

    angular
        .module('app')
        .controller('form_order_controller', form_order_controller);

    form_order_controller.$inject = ['$scope', '$http', '$filter', '$route'];

    function form_order_controller($scope, $http, $filter, $route) {
        $scope.$route = $route;
        $scope.sites = null;
        $scope.model = {
            $sendingData: false,
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
            $scope.model.number = "" + $filter('date')(new Date(), "dd-MM/yy") + "-0";
            $scope.model.order = null;
        };

        $scope.SubmitOrder = function () {
            $scope.message = null;
            if ($scope.OrderForm.$invalid || !$scope.model.order) {
                $scope.message = "Заполните все поля";
                return;
            }

            var data = new FormData();
            data.append("siteId", $scope.model.siteId);
            data.append("number", $scope.model.number);
            data.append("customerid", $scope.model.customerid);

            // files
            data.append("Order", $scope.model.order);

            $scope.model.$sendingData = true;
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
                $scope.model.$sendingData = false;
            }).error(function (data, status, headers, config) {
                console.error("SubmitOrder. Error responce:", status, data);
                $scope.message = "Возникла ошибка при отправке";
                $scope.model.$sendingData = false;
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

        $scope.Init = function () {
            $scope.ClearModel();
            $scope.GetSites();
        };
    }
})();
