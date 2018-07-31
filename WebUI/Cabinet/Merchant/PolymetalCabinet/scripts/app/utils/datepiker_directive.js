(function () {
    'use strict';

    angular
        .module('app')
        .directive('datepicker', function () {
            return {
                restrict: 'E',
                template: `<input type="text" ng-model="dpValue | date : 'dd.MM.yyyy'"/>`,
                replace: true,
                scope: {
                    dpValue: '=',
                    dpChanged: '&'
                },
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
})();