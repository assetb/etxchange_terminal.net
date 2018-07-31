(function () {
    'use strict';

    angular
        .module('app')
        .controller('general_statistic', general_statistic);

    general_statistic.$inject = ['$scope', '$http'];

    function general_statistic($scope, $http) {
        $scope.google = {
            chat: null,
            data: null
        };

        $scope.model = null;

        // methods
        $scope.GetModel = function () {
            $http.get(baseUrl + "/api/analytic/general", { params: { customerId: CUSTOMER_ID } }).success(function (data) {
                $scope.model = data;
                $scope.UpdateData();
                $scope.DrawChart();
            });
        };

        $scope.InitData = function () {
            $scope.google.data = new google.visualization.DataTable();
            $scope.google.data.addColumn("string", "Name");
            $scope.google.data.addColumn("number", "Number");
        };

        $scope.UpdateData = function () {
            $scope.InitData();
            if ($scope.model != null) {
                $scope.google.data.addRow(["Состоявшиеся", $scope.model.finished]);
                $scope.google.data.addRow(["Не состоявшиеся", $scope.model.notHeld]);
                $scope.google.data.addRow(["Ожидается", $scope.model.expected]);
            };
        };

        $scope.DrawChart = function () {
            $scope.google.chat.draw($scope.google.data, { is3D: true, legend: { position: 'bottom' } });
        };

        $scope.Init = function () {
            if (google != null) {
                console.log(google);
                $scope.InitData();
                $scope.google.chat = new google.visualization.PieChart(document.getElementById("chartdiv"));
                $scope.GetModel();
            }
        };
    }
})();
