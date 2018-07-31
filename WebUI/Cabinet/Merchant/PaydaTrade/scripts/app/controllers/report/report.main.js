require([
    "angular", "alta-api", "datepicker"
], function (ng) {
    var main = ng.module("altaik.report.main", ["alta.service.api", "alta.directive.datepicker"]);
    main.controller("TechSpecCtrl", ["$window", "$scope", '$httpParamSerializer', "altaApi", function ($window, $scope, $httpParamSerializer, altaApi) {
        $scope.urlToFile = $window.globalSettings.urlToApp + "/api/document-formation/report-tech-spec";
        $scope.model = {
            siteId: 0,
            statusId: 1,
            startDate: new Date(),
            endDate: new Date(),
            sortMode: 0,
            sortColumnName: null,
            dateFilterType : "auctionDate"
        };

        $scope.setOrder = function (columnName) {
            $("th.active-head").removeClass("arrow-down").removeClass("arrow-up");
            if (columnName == null) {
                $scope.model.sortMode = 0;
                $scope.model.sortColumnName = null;
            } else {

                if ($scope.model.sortColumnName === columnName) {
                    $scope.model.sortMode = $scope.model.sortMode == 1 ? -1 : 1;
                } else {
                    $scope.model.sortMode = 1;
                    $scope.model.sortColumnName = columnName;
                }

                $("th#" + columnName).addClass($scope.model.sortMode == 1 ? "arrow-up" : "arrow-down");
            }
            $scope.update();
        }

        $scope.getMarkets = function () {
            altaApi.market.query({}, function (responce) {
                $scope.markets = responce;
                $scope.update();
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
            window.open($scope.urlToFile + "?" + $httpParamSerializer($scope.model), "Скачивание отчета");
        };


        $scope.report = null,

		$scope.update = function () {
		    $scope.updateTime();
		    $scope.report = [];
		    $scope.$submiting = true;

		    altaApi.report.getTechSpec($scope.model, function (responce) {
		        if (responce.code == 200) {
		            $scope.report = responce.data;
		        } else {
		            console.error(responce.descroption);
		        }
		        $scope.$submiting = false;
		    }, function (responce) {
		        notification.alert("Ошибка(" + responce.status + ")", "Обтатитесь к брокеру");
		        $scope.$submiting = false;
		    });
		};

        $scope.init = function () {
            $scope.getMarkets();
        }

    }]);
    ng.bootstrap($("#container_application"), ['altaik.report.main']);
});