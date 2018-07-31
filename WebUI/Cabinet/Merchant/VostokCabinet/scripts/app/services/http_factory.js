/// <reference path="../../angular.js" />
(function () {
    'use strict';

    angular
        .module('app')
        .factory('http_factory', http_factory);

    http_factory.$inject = ['$http'];

    function http_factory($http) {
        var $service = {
            $messages: {},
            $lastIdMessage: 0,
            Send: Send,
            Get: Get,
            Post: Post,
            get: Get,
            post: Post,
        };

        function Post(url, data, success, error, config) {
            if (config == null) {
                config = {};
            }
            config["transformRequest"] = angular.identity;
            config["headers"] = { 'Content-Type': undefined };
            config["data"] = data;

            if (config.isMultiple) {
                var formData = new FormData();
                if (config.data) {
                    angular.forEach(config.data, function (value, key) {
                        formData.append(key, value);
                    });
                }
                if (config.files) {
                    angular.forEach(config.files, function (value, key) {
                        formData.append(key, value);
                    });
                }
                config.files = null;
                config.data = formData;
            }
            Send("POST", url, config, success, error);
        };

        function Get(url, config, success, error) {
            if (config == null) {
                config = {};
            }
            Send("GET", url, config, success, error);
        };

        function Send(method, url, config, success, error) {
            var id = ++$service.$lastIdMessage;
            var keyMessage = method + " " + url;
            $service.$messages[keyMessage] = id;

            config["method"] = method,
            config["url"] = url;
            config["$id"] = id;
            config["$key"] = keyMessage;
            config["$success"] = success;
            config["$error"] = error;

            $http(config).success(function (data, status, headers, config) {
                console.log('Success message ' + config.$key + ' (' + config.$id + ')');
                if (config.async || config.$id == $service.$messages[config.$key]) {
                    console.log("succes(" + status + ")message: ", data, config);
                    config.$success && config.$success(data, status, headers, config);
                }
            }).error(function (data, status, headers, config) {
                if (config.$id == $service.$messages[config.$key]) {
                    config.$error && config.$error(data, status, headers, config);
                }
            });
        }

        return $service;
    }
})();