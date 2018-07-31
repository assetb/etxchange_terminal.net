require([
    "angular",
    "alta-api",
    "supplier-api", ,
    "file-input"
], function (angular) {
    var app = angular.module("app.profile.Document", [
       "app.directives.fileInput",
       "alta.service.SupplierApi",
       "alta.service.api"
    ]);

    app.controller("DocumentCtrl", ["$window", "$scope", "altaApi", "SupplierApi",
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
    ]);

    app.controller("AddDocumentCtrl", ["$window", "$scope", "altaApi",
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
                    if(element)
                        element.prop("disabled", false);
                    $window.location.href = $("a#GoToBack").attr("href");
                }, function (responce) {
                    if (element)
                        element.prop("disabled", false);
                    notification.showErrApplication(responce.status);
                });
            };
        }
    ]);

    angular.bootstrap($("#render_body"), ["app.profile.Document"]);
});
