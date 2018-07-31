require([
"angular",
"alta-api",
"file-input",
"app.services.DocumentFormation",
"supplier-api",
], function (ng) {
    var procuratoryModule = ng.module("ProcuratoryModule", ["alta.service.api", "app.directives.fileInput", "app.services.DocumentFormation", "alta.service.SupplierApi"]);

    var ProcuratoryCtrl = function ($scope, $window, altaApi, DocumentFormationService, SupplierApi) {
        $scope.df = DocumentFormationService;
        $scope.message = null;
        $scope.auctionId = null;
        $scope.auction = null;
        $scope.supplierOrder = null;
        $scope.lots = [];
        $scope.autoCouting = true;
        $scope.step = 0;

        $scope.nextStep = function () {
            $scope.step++;
        }

        $scope.prevStep = function () {
            $scope.step--;
        }

        $scope.getAuction = function () {
            altaApi.auction.get({ auctionId: $scope.auctionId }, function (responce) {
                $scope.auction = responce;
                ng.forEach($scope.auction.lots, function (lot) {
                    if ($scope.supplierOrder.lots.some(function (supplierLot) {
                        return lot.id === supplierLot.id;
                    })) {
                        $scope.lots.push(lot);
                    }
                });
            });
        };

        $scope.getMinimalSum = function () {
            var sum = 0;
            ng.forEach($scope.lots, function (lot) {
                sum += lot.Sum;
            });
            return sum;
        }

        $scope.getStartSum = function () {
            var sum = 0;
            ng.forEach($scope.lots, function (lot) {
                sum += (lot.Quantity * lot.Price);
            });
            return sum;
        }

        $scope.getDonwloadTempalte = function ($event) {
            $($event.target).prop('disabled', true);

            $scope.df.generateProcuratory($scope.auctionId, $scope.autoCouting, $scope.lots,
                function (responce) {
                    $($event.target).prop('disabled', false);
                }, function (responce) {
                    $($event.target).prop('disabled', false);
                    notification.showErrApplication(responce.status);
                });
        }

        $scope.generateTechSpec = function (auctionId, lotId, $event) {
            var element = null;
            if ($event != null && $event.target != null) {
                element = $($event.target);
                element.prop("disabled", true);
            }
            $scope.df.generateTechSpec(auctionId, lotId,
                function () {
                    if (element)
                        element.prop("disabled", false);
                },
                function (responce) {
                    if (element)
                        element.prop("disabled", false);
                });

        };

        $scope.getSupplierOrder = function () {
            SupplierApi.supplier.getSupplierOrder({
                auctionId: $scope.auctionId,
            }, function (responce) {
                $scope.supplierOrder = responce;
                $scope.getAuction();
            });
        }

        $scope.submit = function ($event) {
            var element = $("button[type=\"submit\"]", $event.target);
            element.prop("disabled", true);

            SupplierApi.supplier.putProcuratory({
                auctionId: $scope.auctionId
            }, {
                form: {
                    lots: $scope.lots
                },
                files: {
                    template: $scope.$$templateProcuratiry,
                    scan: $scope.$$originFile
                }
            }, function (responce, headers, status) {
                window.location.href = $('#backUrl').attr('href');
                notification.showInfo("Поручение успешно отправлено");
                element.prop("disabled", false);
            }, function (responce) {
                notification.showWarn("Произошла ошибка при отправки поручения");
                element.prop("disabled", false);
            });
        };

        $scope.init = function (auctionId) {
            $scope.auctionId = auctionId;
            $scope.getSupplierOrder();
        };
    };

    ProcuratoryCtrl.$inject = ["$scope", "$window", "altaApi", "DocumentFormationService", "SupplierApi"];
    procuratoryModule.controller("ProcuratoryCtrl", ProcuratoryCtrl);

    ng.bootstrap($("#main_container"), ["ProcuratoryModule"]);
});