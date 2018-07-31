require([
    "angular",
    "alta-api",
    "supplier-api",
    "file-input"
], function (angular) {
    var app = angular.module("app.profile.companyProducts", [
       "app.directives.fileInput",
       "alta.service.api",
       "alta.service.SupplierApi"
    ]);

    app.controller('ProductCtrl', ["$window", "$scope", "altaApi", "SupplierApi",
            function ($window, $scope, altaApi, SupplierApi) {
                $scope.URL_ARCHIVE = baseUrl + '/api/archive/file';
                $scope.model = {
                    products: []
                };

                $scope.getProducts = function () {
                    $scope.model.products = [];
                    SupplierApi.supplier.getProducts({}, function (data) {
                        $scope.model.products = data;
                    })
                };

                $scope.remove = function (productId) {
                    SupplierApi.supplier.deleteProduct({ productId: productId }, function (data) {
                        $scope.updateModel();
                    });
                };

                $scope.updateModel = function () {
                    $scope.getProducts();
                }
            }
    ]);

    app.controller("AddProductCtrl", ["$scope", "$window", "SupplierApi",
        function ($scope, $window, SupplierApi) {
            $scope.model = {
                name: null,
                description: null,
                file: null,
            }
            $scope.submit = function ($event) {
                var button = null;

                if ($event != null && $event.target != null) {
                    button = $("button[type=\"submit\"]", $event.target).prop("disabled", true);
                }

                if (!$scope.model.name) {
                    $scope.model.$errorMessage = "Заполните форму.";
                    return;
                }
                var data = new FormData();
                data.append("name", $scope.model.name);
                data.append("description", $scope.model.description);
                // files
                data.append("file", $scope.model.file);

                $scope.isSubmiting = true;
                SupplierApi.supplier.createProduct({},
                data, function () {
                    if (button)
                        button.prop("disabled", false);
                    notification.showInfo("Товар\услуга добавлен.");
                    $window.location.href = $("a#GoToBack").attr("href");
                }, function () {
                    if (button)
                        button.prop("disabled", false);
                    notification.showErrApplication();
                });
            };
        }
    ]);

    angular.bootstrap($("#render_body"), ["app.profile.companyProducts"]);
});
