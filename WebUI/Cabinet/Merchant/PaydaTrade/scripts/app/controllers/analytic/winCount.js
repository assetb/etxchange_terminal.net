require([
        "angular",
        "pagination",
        "altatender-api",
        "angular-filter"
    ],
    function(ng) {
        var winCountModule = ng.module("winCountModule",
            ["angular.filter", "alta.api.altatender", "alta.directive.pagination"]);
        winCountModule.controller("wincountCrtl",
            [
                "$scope", "AltatenderApi", function($scope, api) {
                    $scope.filterForm = {
                        page: 1,
                        count: 20
                    };

                    $scope.countPages = 0;
                    $scope.count = 0;
                    $scope.winsCounts = [];

                    $scope.namesProducts = [];

                    $scope.selecteProduct = function(productName) {
                        $scope.filterForm.name = productName;
                        $scope.namesProducts = [];
                    }

                    $scope.findProducts = function($event) {
                        if ([38, 40].indexOf($event.keyCode) !== -1) {
                            return;
                        }
                        $scope.namesProducts = [];
                        if ([13].indexOf($event.keyCode) !== -1) {
                            return;
                        }

                        api.lots.find({ name: $scope.filterForm.name, count: 10 },
                            function(responce) {
                                $scope.namesProducts = responce.myArray;
                            });
                    };

                    $scope.update = function(page, countItems) {
                        if (page != undefined) {
                            $scope.filterForm.page = page;
                        }
                        if (countItems != undefined) {
                            $scope.filterForm.count = countItems;
                        }

                        api.winCount.get($scope.filterForm,
                            function(responce) {
                                $scope.winsCounts = responce.winsCounts;
                            });

                        api.winCount.count($scope.filterForm,
                            function(responce) {
                                $scope.count = responce.countArray[0];
                                $scope.countPages = Math.ceil($scope.count / $scope.filterForm.count);
                            });
                    };

                    $scope.init = function() {
                        $scope.update();
                    };
                }
            ]);

        ng.bootstrap($("#renderBody"), ['winCountModule']);
    });