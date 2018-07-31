(function () {
    'use strict';

    angular
        .module('app', ['ngRoute'])
        .config(function ($routeProvider, $windowProvider) {
            var $window = $windowProvider.$get();
            $routeProvider
            .when("/list_orders", {
                templateUrl: $window.globalSettings.applicationPath + "templates/order/form_order.html",
                controller: "list_orders_controller"
                }).when("/list_orders", {
                templateUrl: $window.globalSettings.applicationPath + "templates/order/form_order.html",
                controller: "form_order_controller"
            //}).when("/form_multiple_order", {
            //    templateUrl: $window.globalSettings.applicationPath + "templates/order/form_multiple_order.html",
            //    controller: "form_multiple_order_controller"
            }).otherwise({ redirectTo: '/list_orders' });
        });
})();