require([
    "angular",
    "app.services.CustomerApi",
    "app.services.Archive"
], function (angular) {
    angular.module("app.controllers.profile.Supplier", ["app.services.CustomerApi", "app.services.Archive"])
                .controller("supplier_details_controller", supplier_details_controller);

    supplier_details_controller.$inject = ["$scope", "$http", "$window", "SupplierApi", "ArchiveService"];

    function supplier_details_controller($scope, $http, $window, SupplierApi, ArchiveService) {
        $scope.urlToArchive = $window.globalSettings.urlToArhive;
        $scope.supplierId = null;
        $scope.company = null;
        $scope.products = null;
        $scope.grades = null;
        $scope.commetricalOffers = null;

        $scope.model = {};

        $scope.isLoading = false;

        $scope.getFile = function ($event, id) {
            var btn = $($event.target).attr("disabled", true);
            ArchiveService.getFile(id, function () {
                btn.attr("disabled", false);
            }, function () {
                btn.attr("disabled", false);
            });
        }

        $scope.getFiles = function () {
            $scope.model.files = [];
            SupplierApi.getDocuments({ supplierId: $scope.supplierId },
                function (othersDocuments) {
                    $scope.model.files = othersDocuments;
                }, function (respomce, status) {
                    console.warn("Responce error: " + status);
                });
        }

        $scope.getCompany = function () {
            SupplierApi.getCompany({ supplierId: $scope.supplierId }, function (company) { $scope.company = company; }, function (responce, status) { console.warn("Error responce: " + status); });
            //$http.get(baseUrl + "/api/company/" + $scope.companyid)
            //    .success(function (data) {
            //        $scope.company = data;
            //        if ($scope.company != null) {
            //            $scope.getProducts($scope.company.id);
            //        }
            //    })
            //    .error(function (data) {
            //        $scope.company = null;
            //    });
        };


        $scope.getProducts = function () {
            //$http.get(baseUrl + "/api/product", { "params": { "companyid": id } })
            //    .success(function (data) {
            //        $scope.products = data;
            //    })
            //    .error(function (data) {
            //        $scope.products = null;
            //    });
            $scope.model.products = [];
            SupplierApi.getProducts({ supplierId: $scope.supplierId },
                function (products) {
                    $scope.model.products = products;
                }, function (respomce, status) {
                    console.warn("Responce error: " + status);
                });
        };

        $scope.capthaUUID = null;
        $scope.captcha_res = null;
        $scope.kgdDutyDate = null;
        $scope.unreabilityData = null;
        $scope.nonexeactData = null;

        $scope.generateUUID = function () {
            var d = new Date().getTime();
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,
                function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
                });
        };

        $scope.img = null;
        $scope.UpdateImg = function () {
            if ($scope.capthaUUID == null)
                $scope.capthaUUID = $scope.generateUUID();
            var url = "http://kgd.gov.kz/apps/services/CaptchaWeb/generate?uid=" +
                $scope.capthaUUID +
                "&t=" +
                $scope.generateUUID();
            console.log(url);
            $scope.img = url;

        };

        //$scope.GetData = function () {
        //    $http.post("http://kgd.gov.kz/apps/services/culs-taxarrear-search-web/rest/search", {
        //        "captcha-id": $scope.capthaUUID,
        //        "captcha-user-value": $scope.captcha_res,
        //        "iinBin":$scope.company.bin
        //    }, {
        //        headers: {
        //            "Access-Control-Allow-Origin":"*"
        //        }
        //    })
        //    .success(function (data) {
        //       $scope.kgdDutyDate = data;
        //    })
        //}

        $scope.GetData = function () {
            if ($scope.img == null) {
                alert("Обновите Капчу.");
            } else {
                $http.get(baseUrl + "/api/gosregistry/kgdduty3",
                    {
                        "params": $.param({
                            "captchaId": $scope.capthaUUID,
                            "captchaUserValue": $scope.captcha_res,
                            "bin": $scope.company.bin
                        })
                    })
                    .success(function (data) {
                        $scope.kgdDutyDate = data;
                    });
            }
        };

        $scope.GetUnreabilityData = function () {
            if ($scope.img == null) {
                alert("Обновите Капчу.");
            } else {
                setTimeout(function () { $scope.unreabilityData = "No"; }, 2000);

            }
        };

        $scope.GetNonexeactsData = function () {
            if ($scope.img == null) {
                alert("Обновите Капчу.");
            } else {
                $scope.nonexeactData = "No";
            }
        };

        $scope.init = function (supplierId) {
            $scope.supplierId = supplierId;
            $scope.getCompany();
            $scope.getFiles();
            $scope.getProducts();
        };
    }

    angular.bootstrap($("#contentBody"), ["app.controllers.profile.Supplier"]);
});