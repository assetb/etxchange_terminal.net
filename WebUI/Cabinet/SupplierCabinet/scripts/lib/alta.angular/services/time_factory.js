(function () {
    'use strict';

    angular
        .module('app')
        .factory('time_factory', time_factory);

    time_factory.$inject = ['$http'];

    function time_factory($http) {
        var service = {
            GetDateTime: function () { },
            GetTime: function () { }
        };
        return service;
    }
})();