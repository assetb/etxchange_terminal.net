(function () {
    'use strict';

    angular
        .module('app')
        .service('order_upload_service', order_upload_service);

    order_upload_service.$inject = ['$http'];

    function order_upload_service($http) {
        var service = {
            upload: function (file, url, callbackSuccess, callbackError) {
                var form = new FormData();

                form.append('file', file);
                return $http.post(url, form, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).success(function (data) {
                    callbackSuccess != null && callbackSuccess(data);
                }).error(function (data) {
                    callbackError != null && callbackError(data);
                })
            },
            getData: function () { }
        }
        return service;
    };
})();