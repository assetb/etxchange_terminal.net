require([
    "angular",
    "alta-api",
    "supplier-api",
    "app.services.Company",
    "file-input"
], function (angular) {
    angular.module("app.controllers.profile.CompanyProfile", ["app.directives.fileInput", "alta.service.SupplierApi", "alta.service.api"])
        .controller("DocumentCtrl", ["$window", "$scope", "altaApi", "SupplierApi",
        function ($window, $scope, altaApi, SupplierApi) {
            $scope.urlToArchive = $window.globalSettings.urlToArhive;
            $scope.model = {
                files: [],
                agreements: []
            };

            $scope.removeDocument = function (fileListId, documentId) {
                altaApi.archive.removeDocumentInList({ fileListId: fileListId, documentId: documentId }, {}, function () {
                    $scope.updateModel();
                }, function () {
                    $scope.updateModel();
                });
            };

            $scope.getAgreemets = function () {
                $scope.model.agreements = [];
                SupplierApi.supplier.getContractsDocuments(function (contractsDocuments) {
                    $scope.model.agreements = contractsDocuments;
                }, function () {
                    notification.showErrApplication();
                });
            };

            $scope.getFiles = function () {
                $scope.model.files = [];
                SupplierApi.supplier.getOtherDocuments(
                    function (othersDocuments) {
                        $scope.model.files = othersDocuments;
                    }, function () {
                        notification.showErrApplication();
                    });
            }

            $scope.updateModel = function () {
                $scope.getFiles();
                $scope.getAgreemets();
            };
        }
        ])
        .controller("AddDocumentCtrl", ["$window", "$scope", "altaApi",
        function ($window, $scope, altaApi) {
            $scope.model = {
                $errorMessage: null,
                description: null,
                file: null,
                docTypes: [
                    {
                        id: 22,
                        name: 'Пользовательский'
                    },
                    {
                        id: 21,
                        name: 'Договор'
                    }
                ]
            };

            $scope.submit = function ($event) {
                if (!$scope.model.description) {
                    $scope.model.$errorMessage = "Заполните форму.";
                    return;
                }

                $scope.isSubmiting = true
                var element = null;

                if ($event != null && $event.target != null) {
                    element = $("button[type=\"submit\"]", $event.target).prop("disabled", true);
                }

                var data = new FormData();
                data.append("description", $scope.model.description);
                data.append("file", $scope.model.file);

                altaApi.company.saveDoc({
                    docTypeId: $scope.model.docTypeId
                }, data, function () {
                    notification.showInfo("Документ добавлен.");
                    $scope.isSubmiting = false;
                    if (element)
                        element.prop("disabled", false);
                    $window.location.href = $("a#GoToBack").attr("href");
                }, function (responce) {
                    if (element)
                        element.prop("disabled", false);
                    notification.showErrApplication(responce.status);
                });
            };
        }
        ])
        .controller("CompanyCtrl", ['$window', '$scope', 'altaApi', "CompanyApi", "SupplierApi",
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
        ])
        .controller("CompanyCtrl", ['$window', '$scope', 'altaApi', "CompanyApi", "SupplierApi",
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

    angular.bootstrap($("#render_body"), ["app.controllers.profile.CompanyProfile"]);
});
