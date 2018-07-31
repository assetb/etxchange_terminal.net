
define([
    "angular",
    "foundation-datepicker"
], function (ng) {

    var app = ng.module("alta.directive.datepicker", []);

    app.directive("datepicker", ["$filter", function ($filter) {
        return {
            restrict: 'E',
            template: "<input type=\"text\" placeholder=\"{{dpText}}\" value=\"{{(dpValue | date : 'dd.MM.yyyy') || ''}}\"/>",
            replace: true,
            scope: {
                dpText: "@",
                dpValue: "=",
                dpChanged: "&"
            },
            controller: ["$scope", "$filter", function ($scope, $filter) {
                $scope.getValueString = function () {
                    return $scope.dpValue ? $filter("date")($scope.dpValue, "dd.MM.yyyy") : "";
                }
            }],
            link: function (scope, element, attrs) {
                if (typeof (scope.format) == "undefined") { scope.format = "dd.mm.yyyy" }
                $(element).fdatepicker({ format: scope.format }).on("changeDate", function (ev) {
                    scope.$apply(function () {
                        scope.dpValue = ev.date;
                    });
                    scope.dpChanged && scope.dpChanged();
                })
            }
        }
    }]);
    return app;
});