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
            siteId: 4,
            number: null,
            order: null,
            agreement: null,
            orderOrigin: null,
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
            $scope.model.siteId = 4;
            $scope.model.number = "" + $filter('date')(new Date(), "dd-MM/yy") + "-0";
            $scope.model.order = null;
            $scope.model.agreement = null;
            $scope.model.orderOrigin = null;
            $scope.model.shemes = [];
        };

        $scope.SubmitOrder = function () {
            $scope.message = null;
            if ($scope.OrderForm.$invalid || !$scope.model.order || !$scope.model.agreement) {
                notification.alert("Внимание!", "Заполните все поля");
                return;
            }

            var data = new FormData();
            data.append("siteId", $scope.model.siteId);
            data.append("number", $scope.model.number);

            // files
            data.append("Order", $scope.model.order);
            data.append("Agreement", $scope.model.agreement);
            data.append("OrderOrigin", $scope.model.orderOrigin);

            //file type sheme
            data.append("count_shemes", $scope.model.shemes.length);
            for (var i = 0; i < $scope.model.shemes.length; i++) {
                data.append("Sheme" + i, $scope.model.shemes[i]);
            }

            $scope.model.$sendingData = true;
            $http.post(
            baseUrl + "/api/customer/order",
            data,
            {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
            .success(function (data, status, headers, config) {
                if (data == null) {
                    notification.alert("Внимание!", "Ошибка при сохранении заявки. Обратитесь к своему брокеру.");
                } else {
                    notification.success("Успешно", "Заявка добавлена.");
                    $scope.ClearModel();
                }
                $scope.model.$sendingData = false;
            }).error(function (data, status, headers, config) {
                notification.alert("Внимание!", "Возникла ошибка при отправке");
                $scope.model.$sendingData = false;
            });
        };

        //$scope.GetSites = function () {
        //    $http.get(baseUrl + "/api/auction/sites")
        //    .success(function (data) {
        //        $scope.sites = data;
        //    }).error(function (data, status, headers, config) {
        //        notification.alert("Внимание!", "Не удалось получить список бирж");
        //        $scope.sites = [];
        //    });
        //};

        $scope.Init = function () {
            $scope.ClearModel();
            //$scope.GetSites();
        };
    }
})();
