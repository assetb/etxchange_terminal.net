require([
        "angular",
        "pagination",
        'alta-api',
        "altatender-api",
        "angular-filter"
    ],
    function(ng) {
        var protocolModule = ng.module("ProtocolModule",
            ["angular.filter", "alta.api.altatender", "alta.directive.pagination"]);

        protocolModule.controller("ProtocolCtrl",
            [
                "$scope", "AltatenderApi", function($scope, api) {
                    $scope.form = {
                        page: 1,
                        count: 20,
                        source: null,

                    };
                    $scope.countPages = 0;
                    $scope.methodSearch = 0;
                    $scope.methodsSearch = [
                        {
                            text: "По Названию",
                            queryParam: "name"
                        },
                        {
                            text: "По Организатору",
                            queryParam: "organizer"
                        },
                        {
                            text: "По Источнику",
                            queryParam: "source"
                        },
                        {
                            text: "По Региону",
                            queryParam: "region"
                        },
                        {
                            text: "По Номеру",
                            queryParam: "number"
                        }
                    ];

                    $scope.queryString = "";

                    $scope.sources = [
                        { id: 1, name: 'Гос. закупки' },
                        { id: 2, name: 'Самрук-Казына' }
                    ];

                    $scope.protocols = [];

                    $scope.update = function(page, count) {
                        if (page != undefined) {
                            $scope.form.page = page;
                        }
                        if (count != undefined) {
                            $scope.form.count = count;
                        }

                        var form = {
                            page: $scope.form.page,
                            countItems: $scope.form.count,
                        };

                        form[$scope.methodsSearch[$scope.methodSearch].queryParam] = $scope.queryString;

                        api.protocols.query(form,
                            function(responce) {
                                $scope.protocols = responce;
                            });
                        api.protocols.count(form, function (responce) {
                            $scope.count = responce.countArray[0];
                            $scope.countPages = Math.ceil($scope.count / $scope.form.count);
                        });
                    };

                    $scope.init = function() {
                        $scope.update();
                    };
                }
            ]);

        ng.bootstrap($("#protocol-module"), ["ProtocolModule"]);
    });