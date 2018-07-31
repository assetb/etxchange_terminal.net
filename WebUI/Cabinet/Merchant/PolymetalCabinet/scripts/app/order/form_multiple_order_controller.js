/// <reference path="../services/http_factory.js" />
(function () {
    'use strict';

    angular
        .module('app')
        .controller('form_multiple_order_controller', form_multiple_order_controller);

    form_multiple_order_controller.$inject = ['$window', '$scope', '$filter', 'http_factory'];

    function form_multiple_order_controller($window, $scope, $filter, http) {
        $scope.$urlToArchive = baseUrl + "/api/archive";
        $scope.model = {}

        $scope.update = function () {
            $scope.model = {
                $$countRequestSended: 0,
                $$countRequestSuccess: 0,
                $isSubmiting: false,
                isSelected: false,
                customerId: null,
                orders: [],
                orderOrigin: null,
            };
            $scope.GetOrders();
        };

        $scope.submit = function () {
            $scope.model.$isSubmiting = true;
            var orders = $scope.model.orders.filter(function (order) { return order.selected });
            if (orders.length) {
                $scope.model.$$countRequestSuccess = 0;
                $scope.model.$$countRequestSended = orders.length;
                angular.forEach(orders, function (order) {
                    var fileList = order['<filesList>k__BackingField'];
                    var firstFile = fileList.files[0];
                    var documentRequisite = {
                        section: firstFile.section,
                        market: firstFile.market,
                        type: 23,
                        number: firstFile.number,
                        filesListId: firstFile.filesListId,
                    };
                    http.post(baseUrl + "/api/archive/file",
                        {
                            documentRequisite: angular.toJson(documentRequisite)
                        },
                        function (data, status, headers, config) {
                            config.order.$success = true;
                            $scope.model.$$countRequestSuccess++;
                            if ($scope.model.$$countRequestSended == $scope.model.$$countRequestSuccess) {
                                $scope.update();
                            }
                        },
                        function (data, status, headers, config) {
                            $scope.model.$isSubmiting = false;
                        },
                        {
                            isMultiple: true,
                            files: { file: $scope.model.orderOrigin },
                            order: order,
                            async: true
                        }
                    );
                });
            }
        }

        $scope.GetOrders = function () {
            $scope.model.orders = null;

            http.get(baseUrl + "/api/order", { params: { customerId: $scope.model.customerId, statusId: 1 } },
                function (data) {
                    $scope.model.orders = data;
                },
                function (data, status, headers, config) {
                    console.error("GetOrders. Error responce:", status, data);
                    $scope.model.orders = null;
                });
        };

        $scope.selectOrder = function () {
            var selected = false;
            angular.forEach($scope.model.orders, function (order, filterIndex) {
                if (order.selected)
                    selected = true;
            });
            $scope.model.isSelected = selected;
        }

        $scope.selectAllOrders = function () {
            angular.forEach($scope.model.orders, function (order, filterIndex) {
                order.selected = $scope.model.isSelected;
            });
        }

        $scope.GetFiles = function (fileListId) {
            $scope.files = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId, null,
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
            $scope.customerId = $window.globalSettings.customerId;
            $scope.update();
        };
    }
})();
