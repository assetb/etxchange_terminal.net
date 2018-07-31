require([
    "angular",
    "search",
    "pagination",],

    function (angular) {
        angular.module('searchApp', ["app.services.Search","alta.directive.pagination"])
        .controller('searchController', ['$scope', 'SearchFactory', function ($scope, SearchFactory) {


            $scope.data = {
                method: null,
                availableOptions: [
                    { id: '1', name: 'Наименование' },
                    { id: '2', name: 'БИН' },
                    { id: '3', name: 'Регион' }
                ]
            };

            $scope.model = {
                
            }

            $scope.form = SearchFactory;           
            $scope.form.$url = baseUrl + "/api/supplier/by-params";

            $scope.Init = function () {
                $scope.form.Search();
            };

            $scope.search = function () {
               // $scope.form.scope.params.query = $scope.data.method;
                $scope.form.Search();
                $scope.form.$lastSearchText = $scope.form.params.searchproduct;
            };

            $scope.update = function (page, items) {
                $scope.form.params.page = page;
                $scope.form.params.countItems = items;
                $scope.search();
            };


           
            


            }]);
    angular.bootstrap($("#search_container"), ["searchApp"]);
});


