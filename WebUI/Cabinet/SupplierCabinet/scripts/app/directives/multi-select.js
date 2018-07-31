define(["angular", "foundation-dropdown"],
    function (ng) {
        var multiSelectDirective = ng.module("alta.directive.multiSelect", []);

        multiSelectDirective.service("multiSelect", function () {
            return {
                restrict: 'E',
                template: `<input type="text" placeholder="{{dpText}}" ng-bind="getValueString()"/>`,
                replace: true,
                scope: {
                    dpText: '@',
                    dpValue: '=',
                    dpChanged: '&'
                },
                controller: ['$scope', '$filter', function ($scope, $filter) {
                    $scope.getValueString = function () {
                        return $scope.dpValue ? $filter('date')($scope.dpValue, 'dd.MM.yyyy') : "";
                    }
                }],
                link: function (scope, element, attrs) {
                    if (typeof (scope.format) == "undefined") { scope.format = "dd.mm.yyyy" }
                    $(element).fdatepicker({ format: scope.format }).on('changeDate', function (ev) {
                        scope.$apply(function () {
                            scope.dpValue = ev.date;
                        });
                        scope.dpChanged && scope.dpChanged();
                    })
                }
            }
        });
    });