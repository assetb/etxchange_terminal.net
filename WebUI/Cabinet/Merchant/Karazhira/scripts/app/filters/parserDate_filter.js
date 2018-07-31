(function () {
    'use strict';

    angular
        .module('app')
        .filter('parserDate', function () {
            return function (string) {
                return new Date(string);
            }
        });
})();