define([
    'angular',
    'alta-search'
], function (angular) {
    var app = angular.module('app.search.list', ['alta.service.search']);
    app.controller('list_customers_controller', ['$scope', '$http', 'altaSearch', function ($scope, $http, altaSearch) {
        $scope.form = altaSearch;

        $scope.form.$url = baseUrl + "/api/customer";

        $scope.form.error = function () {
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

    }]);
    return app;
});
