(function () {
    'use strict';

    angular
        .module('app')
        .directive('datepicker', function () {
            return {
                restrict: 'E',
                template: '<input type="date" ng-model="dpValue"/>',
                scope: {
                    dpValue: '=',
                    dpChanged: '&'
                },
                link: function (scope, element, attrs, ngModel) {
                    if (typeof (scope.format) == "undefined") { scope.format = "yyyy-mm-dd" }
                    $("input", element).fdatepicker({ format: scope.format }).on('changeDate', function (ev) {
                        scope.$apply(function () {
                            scope.dpValue = ev.date;
                        });
                        scope.dpChanged && scope.dpChanged();
                    })
                }
            }
        });
})();