$(document).ready(function () {
    var GeneralModule = angular.module("app.controllers.analytic.General", []);
    function GeneralCtrl($scope, $http, $window) {
        var currentDate = new Date();

        $scope.statuses = null;
        $scope.filter = {
            startDate: new Date(new Date().setMonth(currentDate.getMonth() - 1)),
            endDate: currentDate,
        };
        // methods
        $scope.getModel = function () {
            $http.get($window.globalSettings.urlToApp + "/api/customer/analytic/general", { params: $scope.filter }).then(function (responce) {
                $scope.statuses = responce.data;
            });
        };

        $scope.init = function () {
            $scope.getModel();
        };
    };

    GeneralCtrl.$inject = ["$scope", "$http", "$window"];
    GeneralModule.controller("GeneralCtrl", GeneralCtrl);
    angular.bootstrap($("#contentBody"), ["app.controllers.analytic.General"]);
});