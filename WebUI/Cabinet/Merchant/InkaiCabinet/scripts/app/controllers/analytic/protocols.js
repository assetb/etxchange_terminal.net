require([
    "angular",
    "pagination",
    "altatender-api",
    "angular-filter"
], function (ng) {
    var protocolModule = ng.module("ProtocolModule", ["angular.filter", "alta.api.altatender", "alta.directive.pagination"]);

    protocolModule.controller("ProtocolCtrl", ["$scope", "AltatenderApi", function ($scope, api) {
        $scope.form = {
            page: 1,
            count: 20,
            source: null,

        };

        $scope.sources = [
            { id: 1, name: 'Гос. закупки' },
            { id: 2, name: 'Самрук-Казына' }
        ];

        $scope.protocols = [];

        $scope.update = function (page, count) {
            if (page != undefined) {
                $scope.form.page = page;
            }
            if (count != undefined) {
                $scope.form.count = count;
            }

            api.protocols.query($scope.form, function (responce) {
                $scope.protocols = responce;
            });
        };

        $scope.init = function () {
            $scope.update();
        };
    }]);

    ng.bootstrap($("#protocol-module"), ["ProtocolModule"]);
});