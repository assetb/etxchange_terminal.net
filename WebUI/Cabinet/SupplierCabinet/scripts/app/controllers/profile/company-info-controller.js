require([
    "angular",
    "alta-api",
    "app.services.Company",
    "supplier-api"
], function (angular) {
    var app = angular.module("app.profile.companyEditor", ["alta.service.api", "app.services.Company", "alta.service.SupplierApi"]);

    app.controller("CompanyCtrl", ['$window', '$scope', 'altaApi', "CompanyApi", "SupplierApi",
    function ($window, $scope, altaApi, CompanyApi, SupplierApi) {
        $scope.company = null;

        $scope.saveCompany = function ($event) {
            var element = $("button[type=\"submit\"]", $event.target);
            console.log(element);
            element.prop("disabled", true);
            SupplierApi.supplier.updateCompany({}, angular.toJson($scope.company), function (responce) {
                notification.showInfo("Данные о компании изменены.");
                $scope.getCompany();
                element.prop("disabled", false);
            }, function (responce) {
                notification.showWarn("Ошибка при сохранении данных о компании");
                element.prop("disabled", false);
            });
        }

        $scope.getCompany = function () {
            SupplierApi.supplier.getCompany({}, function (data) {
                $scope.company = data;
            }, function (responce) {
                notification.showErrApplication(responce.status);
            });
        };

        $scope.updateModel = function () {
            $scope.getCompany();
        }
    }
    ]);

    angular.bootstrap($("#render_body"), ["app.profile.companyEditor"]);
});
