define([
    "angular"
],
    function (angular) {
        var table = angular.module("app.directives.Table", []);

        table.directive("tableContainer", function () {
            return {
                restrict: "E",
                link: function link(scope, element, attrs, ctrl) {
                    console.log("link tableContainer. ctrl = ", ctrl, "element: ", element);
                },
                transclude: true,
                scope: {},
                template: "<table style=\"width: 100%;\" ng-transclude></table>",
                controller: "tableContainerCtrl",
            };
        });

        table.directive("tableHeaders", function () {
            return {
                link: function link(scope, element, attrs, ctrl) {
                    console.log("link tableHead. ctrl = ", ctrl, "element: ", element);
                },
                replace: true,
                transclude: true,
                require: ["^tableContainer"],
                template: "<thead><tr ng-transclude></tr></thead>",
                controller: "tableHeadCtrl",
                scope: {
                    headers: "=",
                }
            };
        });

        table.directive("tableHead", function () {
            return {
                link: function link(scope, element, attrs, ctrl) {
                    console.log("link tableHead. ctrl = ", ctrl, "element: ", element);
                },
                replace: true,
                require: ["^tableContainer"],
                template: "<th ng-bind=\"title\"></th>",
                scope: {
                    title: "@",
                }
            };
        });

        table.directive("tableRows", function () {
            return {
                require: ["^tableContainer"],
                link: function (scope, element, attrs, ctrl) {
                    console.log("link tableCol. ctrl = ", ctrl, "element: ", element);
                },
                transclude: true,
                replace: true,
                template: "<tbody><tr ng-repeat=\"row in rows\" ng-transclude></tr></tbody>",
                controller: "tableRowsCtrl",
                scope: {
                    rows: "="
                }
            }
        });

        table.directive("tableCol", function () {
            return {
                require: ["^tableRows"],
                link: function (scope, element, attrs, ctrl) {
                    scope.row = scope.$parent.$parent.row;
                    console.log("link tableCol. ctrl = ", ctrl, "element: ", element);
                    switch (scope.type) {
                        case ("date"): {
                            element.removeAttr("ng-bind");
                            element.attr("ng-bind", "row[name] | date : 'shortDate'");
                        } break;
                        case ("currency"): {
                            element.removeAttr("ng-bind");
                            element.attr("ng-bind", "row[name] | currency : '' : 2");
                        } break;
                    }
                },
                transclude: true,
                replace: true,
                template: "<td ng-bind=\"row[name]\"></td>",
                scope: {
                    name: "@",
                    type: "@"
                }
            }
        });


        table.controller("tableContainerCtrl", function ($scope) {

        });

        table.controller("tableHeadCtrl", function ($scope) {

        });

        table.controller("tableColCtrl", function ($scope) {

        });

        table.controller("tableRowsCtrl", function ($scope) {

        });

        return table;
    });