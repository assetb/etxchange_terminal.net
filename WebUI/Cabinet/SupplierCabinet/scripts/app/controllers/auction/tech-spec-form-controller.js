require([
    'angular',
    'alta-api',
    "angular-messages",
    "file-input",
    "app.services.Lot",
    "app.services.DocumentFormation",
    "supplier-api"
], function (angular) {
    var app = angular.module('app.controllers.auction.TechSpecForm', ['alta.service.api', "app.directives.fileInput", "ngMessages", "app.services.Lot", "app.services.DocumentFormation", "alta.service.SupplierApi"]);

    var TechSpecFormCtrl = function ($scope, $http, $window, altaApi, LotApi, DocumentFormationService, SupplierApi) {
        $scope.auctionId = null;
        $scope.lotId = null;
        $scope.lot = null;

        $scope.status = {
            winner: false,
            doNotMatchAmount: false,
            success: false
        };

        $scope.getLot = function () {
            LotApi.get({ lotId: $scope.lotId }, function (lot) {
                $scope.lot = lot;
                getFinalReports();
            });
        };

        function getFinalReports() {
            SupplierApi.supplier.getFinalReports(
                {
                    auctionId: $scope.auctionId
                }, function (finalReports) {
                    angular.forEach(finalReports, function (finalReport) {
                        if (finalReport.lotId === $scope.lot.Id) {
                            $scope.status.winner = true;
                            $scope.lot._winner = true;
                            $scope.lot._endSum = finalReport.finalPriceOffer;
                            return false;
                        }
                    });
                });
        }

        $scope.getCurrentSum = function () {
            var curSum = 0;
            if ($scope.lot != null) {
                angular.forEach($scope.lot.LotsExtended, function (lotEx) {
                    curSum += lotEx.endsum;
                });
            }
            return curSum;
        }

        $scope.isCurrectSums = function () {
            return $scope.lot._endSum == $scope.getCurrentSum();
        }

        $scope.generateTemplate = function () {
            DocumentFormationService.generateTechSpec($scope.auctionId, $scope.lotId);
        }

        $scope.submit = function ($event) {
            var form = $($event.target);
            var button = $("button[type=\"submit\"]", form);

            switch (form.attr("id")) {
                case ("formTable"): {
                    $scope.updateLotExtended(button);
                } break;
                case ("formUseTemplate"): {
                    $scope.updateLotExtendedUseTemplate(button);
                } break;
                default: {
                    notification.showWarn("Ошибка приложения.");
                }
            }
        };

        $scope.updateLotExtended = function (element) {
            element.prop("disabled", true);

            if (!$scope.isCurrectSums()) {
                $scope.status.doNotMatchAmount = true;
                element.prop("disabled", false);
                notification.showWarn("Суммы не сходятся. Разница: " + $scope.getCurrentSum() - $scope.lot._endSum);
                return;
            }

            SupplierApi.supplier.updateTechSpec({
                auctionId: $scope.auctionId,
                lotId: $scope.lot.Id
            }, $.param({ lotExtepdeds: $scope.lot.LotsExtended }),
           function (responce) {
               $scope.status.success = true;
               notification.showInfo("техническмя спецификая успешно обновлена.");
               element.prop("disabled", false);
               $("#backUrl").click();
           }, function (responce) {
               switch (responce.status) {
                   case (400): {
                       notification.showWarn("Техническая спецификация уже заполнена.");
                   } break;
                   case (403): {
                       notification.showWarn("У Вас не прав на изменение тех. спецификации.", "Доступ запрешен!");
                   } break;
                   case (404): {
                       notification.showWarn("Не удалось найти тех. спецификацию.");
                   } break;
                   default: {
                       notification.showErrApplication();
                   }
               }
               element.prop("disabled", false);
           });
        }

        function goToBack() {

        }

        $scope.updateLotExtendedUseTemplate = function (element) {
            element.prop("disabled", true);

            SupplierApi.supplier.updateTechSpecUseTemplate({
                auctionId: $scope.auctionId,
                lotId: $scope.lot.Id
            }, {
                file: $scope.lot.$$techSpecFile
            }, function (data, headers, status) {
                if (status == "201") {
                    //$scope.Init($scope.auctionId);
                }
                element.prop("disabled", false);
            }, function (responce) {
                switch (responce.status) {
                    case (400): {
                        notification.showWarn("Неверно запонен шаблон технической спецификации. Возможно сумма указанная в шаблоне не соответсвует финальной сумме.");
                    } break;
                    case (403): {
                        notification.showWarn("У Вас не прав на изменение тех. спецификации.", "Доступ запрешен!");
                    } break;
                    case (404): {
                        notification.showWarn("Не удалось найти техническую спецификацию.");
                    } break;
                    default: {
                        notification.showErrApplication();
                    }
                }
                element.prop("disabled", false);
            });
        }

        $scope.automaticize = function ($event) {
            var element = $($event.target).prop("disabled", true);
            var difference = $scope.getCurrentSum() - $scope.lot._endSum;
            var diffItem = difference / $scope.lot.LotsExtended.length;
            angular.forEach($scope.lot.LotsExtended, function (lotEx) {
                lotEx.endsum -= diffItem;
                lotEx.endprice -= diffItem / lotEx.quantity;
            });
            notification.showInfo("Цены расставлены.");
            var element = $($event.target).prop("disabled", false);
        };



        $scope.init = function (auctionId, lotId) {
            $scope.auctionId = auctionId;
            $scope.lotId = lotId;
            $scope.getLot();
        };
    };

    TechSpecFormCtrl.$inject = ['$scope', '$http', "$window", 'altaApi', "LotApi", "DocumentFormationService", "SupplierApi"];
    app.controller("TechSpecFormCtrl", TechSpecFormCtrl);

    angular.bootstrap($("#controller_container"), ["app.controllers.auction.TechSpecForm"]);
});
