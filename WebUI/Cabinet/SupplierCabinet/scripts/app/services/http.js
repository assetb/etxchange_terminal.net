/// <reference path="../../requierjs/require.js" />
/// <reference path="../../angular/angular.js" />
define([
'angular',
], function (angular) {
    'use strict';
    var app = angular.module('alta.service.http', []);

    app.factory('altaHttp', ['$http',
            function ($http) {
                var $service = {
                    $messages: {},
                    $lastIdMessage: 0,
                    send: Send,
                    get: Get,
                    post: Post,
                    del: deleteRequest,
                };

                function deleteRequest(url, config, success, error) {

                    if (config == null) {
                        config = {};
                    }
                    config["transformRequest"] = angular.identity;
                    config["headers"] = { 'Content-Type': undefined };
                    Send("DELETE", url, config, success, error);
                }

                function Post(url, data, success, error, config) {
                    if (config == null) {
                        config = {};
                    }
                    config["transformRequest"] = angular.identity;
                    config["headers"] = { 'Content-Type': undefined };
                    config["data"] = data;
                    Send("POST", url, config, success, error);
                };

                function Get(url, config, success, error) {
                    Send("GET", url, config, success, error);
                };

                function SuccessFuntion(responce) {
                    var data = responce.data,
                        status = responce.status,
                        headers = responce.headers,
                        config = responce.config;

                    console.log('Success message ' + config.$key + ' (' + config.$id + ')');
                    if (config.$id == $service.$messages[config.$key]) {
                        console.log(config.$key + " (" + config.$id + ") is last message");
                        config.$success && config.$success(data, status, headers, config.$config);
                    }
                };

                function ErrorFunction(responce) {
                    var data = responce.data,
                        status = responce.status,
                        headers = responce.headers,
                        config = responce.config;
                    if (config.$id == $service.$messages[config.$key]) {
                        config.$error && config.$error(data, status, headers, config.$config);
                    }
                }

                function Send(method, url, config, success, error) {
                    if (config == null) {
                        config = {};
                    }
                    var id = ++$service.$lastIdMessage;
                    var keyMessage = method + " " + url;
                    $service.$messages[keyMessage] = id;

                    config["method"] = method,
                    config["url"] = url;
                    config["$id"] = id;
                    config["$key"] = keyMessage;
                    config["$success"] = success;
                    config["$error"] = error;

                    var req = $http(config).then(SuccessFuntion, ErrorFunction);
                }

                return $service;
            }
    ]);

    return app;
});