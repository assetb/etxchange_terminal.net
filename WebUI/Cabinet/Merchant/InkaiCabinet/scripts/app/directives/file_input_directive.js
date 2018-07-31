(function () {
    'use strict';

    angular
        .module('app')
        .directive('fileInput', ['$window', '$parse', function ($window, $parse) {
            return {
                restrict: 'E',
                template: "<div class=\"row collapse postfix-radius\">" +
                        "<input type=\"file\" class=\"hide\" id=\"{{guid}}\" ng-disapled=\"fDisabled\" ng-required=\"fRequired\" accept=\"{{fAccept}}\" />" +
                        "<div class =\"small-9 columns\">" +
                            "<input readonly type=\"text\" placeholder=\"{{fText}}\" ng-model=\"fValue.name\"/>" +
                        "</div>" +
                        "<div class=\"small-3 columns\">" +
                            "<label for=\"{{guid}}\" class=\"button info postfix\">Обзор...</label>" +
                        "</div>" +
                    "</div>",
                scope: {
                    fValue: '=',
                    fRequired: '@',
                    fAccept: '@',
                    fText: '@',
                    fDisabled: '=',
                    fChange: '&'
                },
                controller: ['$scope', function ($scope) {
                    $scope.$watch('fValue', function () {
                        $scope.fChange && $scope.fChange({ value: $scope.fValue });
                    });
                }],
                link: function link(scope, element, attrs) {
                    scope.guid = (function () {
                        function s4() {
                            return Math.floor((1 + Math.random()) * 0x10000)
                              .toString(16)
                              .substring(1);
                        };
                        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                          s4() + '-' + s4() + s4() + s4();
                    })();

                    element.bind('change', function () {
                        scope.$apply(function () {
                            scope.fValue = $('input[type="file"]', element).prop('files')[0];
                        });
                    });
                }
            };
        }]);
})();