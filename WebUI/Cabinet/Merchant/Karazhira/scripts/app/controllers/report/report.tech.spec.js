/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular.js" />
define(["angular", "alta-api", "datepicker"], function(ng){
	var app = ng.module("altaik.report.tech.spec", ["alta.service.api", "alta.directive.datepicker"]);

	app.controller("TechSpecCtrl", ["$window", "$scope", "altaApi", function ($window, $scope, altaApi) {
		$scope.model = {
			startDate: new Date(),
			endDate: new Date()
		};

		$scope.getUrl = function () {
		    return $scope.urlToFile + "?startDate=" + $scope.model.startDate.toISOString() + "&endDate=" + $scope.model.endDate.toISOString();
		};

		$scope.urlToFile = $window.globalSettings.urlToApp + "/api/report/tech_spec/generate";

		$scope.report = null,

		$scope.update = function () {
			altaApi.report.getTechSpec($scope.model, function (responce) {
				if (responce.code == 200) {
					$scope.report = responce.data;
				} else {
					console.error(responce.descroption);
				}
			});
		}

	}]);
});