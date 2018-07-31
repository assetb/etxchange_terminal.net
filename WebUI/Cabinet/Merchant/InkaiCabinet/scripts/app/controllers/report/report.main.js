/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular.js" />
require([
    "angular", "alta-api", "datepicker"
], function (ng) {
    var main = ng.module("altaik.report.main", ["alta.service.api", "alta.directive.datepicker"]);
    main.controller("TechSpecCtrl", ["$window", "$scope", '$httpParamSerializer', "altaApi", function ($window, $scope, $httpParamSerializer, altaApi) {
        $scope.model = {
            customerId: $window.globalSettings.customerId,
            siteId: 0,
            statusId: 1,
            startDate: new Date(),
            endDate: new Date()
        };

        $scope.getMarkets = function () {
            altaApi.market.query({}, function (responce) {
                $scope.markets = responce;
            });
        };

        $scope.updateTime = function () {
            var startDate = $scope.model.startDate;
            startDate.setHours(0, 0, 0);
            var endDate = $scope.model.endDate;
            endDate.setHours(23, 59, 59);
            $scope.model.startDate = startDate;
            $scope.model.endDate = endDate;
        }

        $scope.generateUrl = function () {
            $scope.updateTime();
            return $scope.urlToFile + "?" + $httpParamSerializer($scope.model);
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
            $scope.getMarkets();
        }

    }]);
    ng.bootstrap($("#container_application"), ['altaik.report.main']);
});