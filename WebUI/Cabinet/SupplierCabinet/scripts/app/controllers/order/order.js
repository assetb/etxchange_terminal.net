require([
    "angular",
    "supplier-api",
    "auction-api",
    "file-input",
    "app.services.DocumentFormation"
], function (ng) {
    var applicationOrder = ng.module("alta.controller.order", ["alta.service.AuctionApi", "alta.service.SupplierApi", "app.services.DocumentFormation", "app.directives.fileInput"]);

    applicationOrder.controller("OrderCtrl", ["$scope", "$window", "DocumentFormationService", "SupplierApi", "AuctionApi", function ($scope, $window, DocumentFormationService, SupplierApi, AuctionApi) {
        $scope.files = {};
        $scope.selectedDocs = {};
        $scope.qualifications = [];

        $scope.getFiles = function (filesListId) {
            SupplierApi.supplier.getOtherDocuments(function (docs) {
                $scope.docs = docs;
            }, function () {
                notification.showWarn("Не удалось загрузить файлы компании. Повторите попытку позже или обратитесь к брокеру.");
            });
        };

        $scope.getDonwloadTempalte = function ($event) {
            var element = $($event.target);

            element.prop("disabled", true);

            DocumentFormationService.generateSupplierOrder($scope.auctionId, $scope.supplierOrder.lots.map(function (lot) { return lot.Id }) , function () {
                element.prop("disabled", false);
            }, function () {
                element.prop("disabled", false);
            });
        }

        $scope.submit = function ($event) {
            var element = $("button[type=\"submit\"]", $event.target).prop("disabled", true);

            SupplierApi.supplier.putSupplierOrder(
                {
                    auctionId: $scope.auctionId
                }, {
                    form: $scope.selectedDocs,
                    files: $scope.files
                }, function (responce) {
                    notification.showInfo("Заявка успешно отправлена на обработку.");
                    window.location.href = $('#backUrl').attr('href');
                }, function (responce) {
                    switch (responce.status) {
                        case (403): {
                            notification.showWarn("У Вас нет прав на выполнение этого действия.", "Доступ запрещен!");
                        } break;
                        default: {
                            notification.showWarn("Не удалось отправить данные. Повторите попытку позже или обратитесь к брокеру.", "Внутреняя ошибка программы.");
                        }
                    }
                    element.prop("disabled", false);
                });
        }

        $scope.getSupplierOrder = function () {
            $scope.supplierOrder = null;
            SupplierApi.supplier.getSupplierOrder({
                auctionId: $scope.auctionId
            }, function (supplierOrder) {
                $scope.supplierOrder = supplierOrder;
                $scope.getQualifications();
            }, function (responce) {
                switch (responce.status) {
                    case (404): {
                        notification.showWarn("Заявки на участие не найдена.");
                    } break;
                    default: {
                        notification.showErrApplication();
                    }
                };
            });
        }

        $scope.getQualifications = function () {
            AuctionApi.qualification.query({ auctionId: $scope.auctionId },
                function (qualifications) {
                    $scope.qualifications = qualifications;
                }, function () {
                    notification.showErrApplication();
                });
        }

        $scope.init = function (auctionId) {
            $scope.auctionId = auctionId;
            $scope.getSupplierOrder();
            $scope.getFiles();
        }
    }]);

    ng.bootstrap($("#main_container"), ["alta.controller.order"]);
});