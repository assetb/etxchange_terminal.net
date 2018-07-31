require([
    "angular",
    "app.services.CustomerApi"
], function (angular) {
    angular.module("app.controllers.profile.Main", ["app.services.CustomerApi"])
    .controller("SupplierInfoCtrl", ["$scope", "SupplierApi", function ($scope, SupplierApi) {
        $scope.supplierId = null;

        $scope.getSupplierInfo = function () {
            SupplierApi.getCompany({ supplierId: $scope.supplierId }, function (supplier) { $scope.company = supplier; }, function (responce, status) { console.warn("Error responce: " + status); });
        }

        $scope.init = function (supplierId) {
            $scope.supplierId = supplierId;
            $scope.getSupplierInfo();
        }
    }])
     .controller("SupplierDocumentCtrl", ["$scope", "$window", "SupplierApi", function ($scope, $window, SupplierApi) {
         $scope.urlToArchive = $window.globalSettings.urlToArhive;
         $scope.model = {
             files: [],
         };

         $scope.getFiles = function () {
             $scope.model.files = [];
             SupplierApi.getDocuments({ supplierId: $scope.supplierId },
                 function (othersDocuments) {
                     $scope.model.files = othersDocuments;
                 }, function (respomce, status) {
                     console.warn("Responce error: " + status);
                 });
         }

         $scope.init = function (supplierId) {
             $scope.supplierId = supplierId;
             $scope.getFiles();
         };
     }]).controller("SupplierProductCtrl", ["$scope", "$window", "SupplierApi", function ($scope, $window, SupplierApi) {
         $scope.urlToArchive = $window.globalSettings.urlToArhive;
         $scope.model = {
             files: [],
         };

         $scope.getProducts = function () {
             $scope.model.products = [];
             SupplierApi.getProducts({ supplierId: $scope.supplierId },
                 function (products) {
                     $scope.model.products = products;
                 }, function (respomce, status) {
                     console.warn("Responce error: " + status);
                 });
         }

         $scope.init = function (supplierId) {
             $scope.supplierId = supplierId;
             $scope.getProducts();
         };
     }]);

    angular.bootstrap($("#contentBody"), ["app.controllers.profile.Main"]);
});