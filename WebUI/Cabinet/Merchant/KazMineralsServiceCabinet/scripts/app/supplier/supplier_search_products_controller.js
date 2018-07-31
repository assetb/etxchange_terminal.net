(function () {
    'use strict';

    angular
        .module('app')
        .controller('supplier_search_products_controller', supplier_search_products_controller);

    supplier_search_products_controller.$inject = ['$scope', '$http', 'search_factory'];

    function supplier_search_products_controller($scope, $http, search_factory) {
        $scope.form = search_factory;

        $scope.form.$url = baseUrl + "/api/supplier/with-products";

        $scope.form.error = function (data, status) {
            console.error("Error request. Status:", status, data);
            $scope.error_message = "Ошибка обработки запроса. Обратитесь в тех поддержку.";
        };

        $scope.search = function () {
            $scope.form.Search();
            $scope.form.$lastSearchText = $scope.form.params.searchproduct;
        };

        $scope.update = function (page, items) {
            $scope.form.params.page = page;
            $scope.form.params.countItems = items;
            $scope.search();
        };

        $scope.MatchText = function (string, findText) {
            if (findText) {
                var re = new RegExp(findText, "i");
                return string.match(re) != null;
            }
            return false;
        };
        $scope.LigtingText = function (string, findText) {
            if (findText) {
                var re = new RegExp(findText, "i");
                string = string.replace(re, '<span style="background-color: #e7e7e7;">$&</span>');
            }
            return string;
        };

        $scope.Init = function () {
            $scope.form.Search();
        };
    }
})();
