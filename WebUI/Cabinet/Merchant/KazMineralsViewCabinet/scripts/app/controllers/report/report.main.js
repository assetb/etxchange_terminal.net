/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular.js" />
require([
    "angular", "alta-api", "datepicker"
], function (ng) {
    var main = ng.module("altaik.report.main", ["alta.service.api", "alta.directive.datepicker"]);
    main.controller("TechSpecCtrl", ["$window", "$scope", "altaApi", function ($window, $scope, altaApi) {
        $scope.model = {
            startDate: new Date(),
            endDate: new Date()
        };

        $scope.updateTime = function () {
            var startDate = $scope.model.startDate;
            startDate.setHours(0, 0, 0);

            var endDate = $scope.model.endDate;
            endDate.setHours(23, 59, 59);
            //var tsCurrentDay = Math.round(startDate.getTime() / 1000);
            //var tsTomorrowDay = tsCurrentDay + (24 * 3600 - 1);

            $scope.model.startDate = startDate;
            $scope.model.endDate = endDate;
        }

        $scope.getUrl = function () {
            $scope.updateTime();
            return $scope.urlToFile + "?startDate=" + $scope.model.startDate.toISOString() + "&endDate=" + $scope.model.endDate.toISOString();
        };

        $scope.urlToFile = $window.globalSettings.urlToApp + "/api/report/tech_spec/generate";

        $scope.report = null,

		$scope.update = function () {
		    $scope.updateTime();
		    altaApi.report.getTechSpec($scope.model, function (responce) {
		        if (responce.code == 200) {
		            $scope.report = responce.data;
		        } else {
		            console.error(responce.descroption);
		        }
		    });
		};

        $scope.init = function () {
            $scope.update();
        }

    }]);
    ng.bootstrap($("#container_application"), ['altaik.report.main']);
});