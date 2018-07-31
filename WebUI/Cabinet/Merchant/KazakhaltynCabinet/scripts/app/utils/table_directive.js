(function () {
    'use strict';

    angular
        .module('app')
        .directive('table_directive', ['$window', function ($window) {
            return {
                scope
                link: function link(scope, element, attrs) {
                },
                restrict: 'EA'
            };

        }])
    .directive('table_directive', function () {
        return {
            link: function link(scope, element, attrs) {
            },
            require: '^table_directive',
            restrict: 'EA'
        };
    });
})();