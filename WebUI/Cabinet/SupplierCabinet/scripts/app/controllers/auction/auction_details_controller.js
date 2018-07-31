define([
    "angular",
    "alta-api",
    "angular-messages",
    "file-input",
    "supplier-api",
    "app.services.DocumentFormation",
    "app.services.Archive"
], function (angular) {
    var app = angular.module("alta.auction.details", [
        "alta.service.api",
        "app.directives.fileInput",
        "alta.service.SupplierApi",
        "ngMessages",
        "app.services.DocumentFormation",
        "app.services.Archive"
    ]);

    app.controller("auction_details_controller", ["$scope", "altaApi", "SupplierApi", "DocumentFormationService", "ArchiveService", function ($scope, altaApi, SupplierApi, DocumentFormationService, ArchiveService) {
        $scope.CONFIRM_SUPPLIER_ORDER = 1;
        $scope.WAITING_ORDER_DEAL = 15;

        $scope.auctionId = null;
        $scope.auction = null;
        $scope.supplierOrder = null;
        $scope.status = {
            confirmSupplierOrder: false,
            waitingOrderDeal: false,
            winner: false
        };

        $scope.GetAuction = function () {
            altaApi.auction.get({ auctionId: $scope.auctionId }, function (auction) {
                $scope.auction = auction;
                $scope.updateStatus();

                getFinalReports();
            }, function (responce) {
                notification.showWarn("Произошла ошибка при загрузке деталей аукциона. Повторите попытку позже или обратитесь к брокеру.");
            });
        };

        $scope.updateStatus = function () {
            SupplierApi.supplier.getSupplierOrder({
                auctionId: $scope.auctionId
            }, function (supplierOrder) {
                $scope.supplierOrder = supplierOrder;

                if ($scope.auction.statusId == 4) {

                    if (supplierOrder != null && supplierOrder.status != null) {

                        switch (supplierOrder.status.Id) {
                            case ($scope.CONFIRM_SUPPLIER_ORDER): {
                                $scope.status.confirmSupplierOrder = true;
                            } break;
                            case ($scope.WAITING_ORDER_DEAL): {
                                $scope.status.waitingOrderDeal = true;
                            } break;
                        }

                    }

                }
            });
        }

        $scope.generateTemplate = function (lot, $event) {
            var element = null;

            if ($event != null && $event.target != null) {
                element = $($event.target).prop("disabled", true);
            }

            DocumentFormationService.generateTechSpec($scope.auctionId, lot.Id, function (responce) {
                if (element)
                    element.prop("disabled", false);
            }, function (responce) {
                if (element)
                    element.prop("disabled", false);
                notification.showErrApplication(responce.status);
            });
        }

        function getFinalReports() {
            $scope.finalReports = null;

            SupplierApi.supplier.getFinalReports({
                auctionId: $scope.auctionId
            }, function (finalReports) {
                $scope.finalReports = finalReports;
                angular.forEach($scope.auction.lots, function (lot) {
                    angular.forEach($scope.finalReports, function (finalReport) {
                        if (finalReport.lotId === lot.Id) {
                            lot.$$winner = true;
                            $scope.status.winner = true;
                        }
                    });
                });
            }, function (responce) {
                notification.showErrApplication(responce.status);
            });
        };

        $scope.getDocuments = function () {
            $scope.documents = [];
            SupplierApi.supplier.getAuctionDocuments({ auctionId: $scope.auctionId },
                function (documents) {
                    $scope.documents = documents;
                }, function (responce) {
                    notification.showErrApplication(responce.status);
                });
        };

        $scope.donwload = function (documentId, $event) {
            var button = null;
            if ($event != null && $event.target != null) {
                button = $($event.target).prop("disabled", true);
            }
            ArchiveService.getFile(documentId, function () {
                button.prop("disabled", false);
            }, function (responce) {
                button.prop("disabled", false);
                notification.showErrApplication(responce.status);
            });
        };

        $scope.Init = function (auctionId) {
            $scope.auctionId = auctionId;
            $scope.GetAuction();
            $scope.getDocuments();
        };
    }]);
    return app;
});
