require([
            'angular',
            'datepicker',
            'pagination',
            'altatender-api',
], function (ng) {
    var app = ng.module('altatender.search.main', ['alta.directive.datepicker', 'alta.directive.pagination', 'altatender.service.api']);

    app.controller('SearchCtrl', ['$scope', 'AltatenderApiFactory', function ($scope, AltatenderApiFactory) {
        $scope.altatenderApi = null;
        $scope.methods = [];
        $scope.sources = [];
        $scope.regions = [];
        $scope.form = {
            methods: [],
            sources: [4],
            regions: [],
        };
        $scope.data = {};

        $scope.init = function () {
            console.log("Start application");

            $scope.altatenderApi = new AltatenderApiFactory("http://localhost:8084/parserweb");

            $scope.loadCatalogs();
            $scope.update(1, 10);
        };

        $scope.loadCatalogs = function () {
            $scope.altatenderApi.Catalog.getMethods(null, function (responce) {
                if (responce.code != 200) {
                    console.warn(responce.code, ": Error :", responce.description);
                }
                $scope.methods = responce.data;
            });

            $scope.altatenderApi.Catalog.getRegions(null, function (responce) {
                if (responce.code != 200) {
                    console.warn(responce.code, ": Error :", responce.description);
                }
                $scope.regions = responce.data;
            });

            $scope.altatenderApi.Catalog.getSources(null, function (responce) {
                if (responce.code != 200) {
                    console.warn(responce.code, ": Error :", responce.description);
                }
                $scope.sources = responce.data;
            });
        }

        $scope.search = function () {
            $scope.update($scope.form.page, $scope.form.maxCount);
        }

        $scope.update = function (page, items) {
            $scope.form.page = page;
            $scope.form.maxCount = items;

            if ($scope.form.regions[0] == null)
                $scope.form.regions = [];

            if ($scope.form.methods[0] == null)
                $scope.form.methods = [];

            if ($scope.form.sources[0] == null)
                $scope.form.sources = [];

            var responce = $scope.altatenderApi.Search.get($scope.form, function (responce) {
                //if (responce) {
                //    console.warn(responce, ": Error: ", responce.description);
                //    return;
                //}

                $scope.data = responce;
            });
        }
    }]);

    ng.bootstrap($('#search_container'), ['altatender.search.main']);
});