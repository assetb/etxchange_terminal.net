(function () {
    'use strict';

    angular
        .module('app')
        .controller('supplier_list_controller', supplier_list_controller);

    supplier_list_controller.$inject = ['$scope', '$http', 'search_factory'];

    function supplier_list_controller($scope, $http, search_factory) {
        $scope.form = search_factory;

        $scope.form.$url = baseUrl + "/api/supplier";

        $scope.form.error = function (data, status) {
            console.error("Error request. Status:", status, data);
            $scope.error_message = "Ошибка обработки запроса. Обратитесь в тех поддержку.";
        };

        $scope.update = function (page, items) {
            $scope.form.params.page = page;
            $scope.form.params.countItems = items;
            $scope.form.Search();
        };

        $scope.Init = function () {
            $scope.form.Search();
        };
    };
})();
