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
            brokerId: null,
            shemes: [],
            filesSize: 0,
            maxFilesSize: 33224432
        };   

        $scope.brokers = [{
            id: '14271',
            name: 'ТОО "Гермес Бурса"'
        }, {
                id: '14272',
                name: '"ТОО "STEPPE NOMAD"'
            }]

        $scope.SumFileSize = function () {
            var assignment = $scope.model.assignment;
            var specification = $scope.model.specification;
            var suppliers = $scope.model.suppliers;

            var bytes = (assignment ? assignment.size : 0) + (specification ? specification.size : 0) + (suppliers ? suppliers.size : 0);

            //for (var i = 0; i < $scope.model.shemes.length; i++) {
            //    bytes += $scope.model.shemes[i] ? $scope.model.shemes[i].size : 0;
            //}

            $scope.model.filesSize = bytes;
        };

        $scope.ClearModel = function () {
            $("input[type='file']").val("");
            $scope.model.siteId = 6;
            $scope.model.number = "KAZZINC " + $filter('date')(new Date(), "dd-MM/yy") + "-0";
            $scope.model.order = null;
            $scope.model.agreement = null;
            $scope.model.orderOrigin = null;
            $scope.model.brokerId = null;
            $scope.model.shemes = [];
        };

        $scope.SubmitOrder = function () {
            $scope.message = null;
            if ($scope.OrderForm.$invalid || !$scope.model.assignment || !$scope.model.specification || !$scope.model.suppliers) {
                notification.alert("Внимание!", "Заполните все поля");
                return;
            }

            var data = new FormData();
            data.append("siteId", $scope.model.siteId);
            data.append("number", $scope.model.number);

            // files
            data.append("assignment", $scope.model.assignment);
            data.append("specification", $scope.model.specification);
            data.append("suppliers", $scope.model.suppliers);

            //file type sheme
            data.append("count_shemes", $scope.model.shemes.length);
            for (var i = 0; i < $scope.model.shemes.length; i++) {
                data.append("Sheme" + i, $scope.model.shemes[i]);
            }

            $scope.model.$sendingData = true;
            $http.post(
            baseUrl + "/api/customer/kaspyOrder",
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

        $scope.toggle_dom = function (element, button) {
            $(element).slideToggle(function () {
                $(button).text(
                    $(this).is(':visible') ? "Скрыть" : "Показать"
                );
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
