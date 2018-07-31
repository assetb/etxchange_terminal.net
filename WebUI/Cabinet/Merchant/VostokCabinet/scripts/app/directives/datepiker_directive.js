(function () {
    'use strict';

    angular
        .module('app')
        .directive('datepicker', function () {
            return {
                restrict: 'E',
                template: "<input type=\"text\" placeholder=\"{{dpText}}\" value=\"{{(dpValue | date : 'dd.MM.yyyy') || ''}}\"/>",
                replace: true,
                scope: {
                    dpValue: '=',
                    dpChanged: '&'
                },
                link: function (scope, element, attrs) {

                    $(element).fdatepicker({ format: "dd.mm.yyyy" }).on('changeDate', function (ev) {
                        scope.$apply(function () {
                            scope.dpValue = ev.date;
                        });
                        scope.dpChanged && scope.dpChanged();
                    })
                }
            }
        });
})();
