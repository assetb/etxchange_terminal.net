/// <reference path="../../angular.js" />
/// <reference path="../../angular-route.js" />
(function () {
    'use strict';

    angular.module('app', ['ngRoute'])
        .config(function ($windowProvider, $routeProvider) {
            var $window = $windowProvider.$get();
            $routeProvider
                .when("/active", {
                    templateUrl: $window.globalSettings.applicationPath + "templates/auction/list.html",
                })
                .when("/history", {
                    templateUrl: $window.globalSettings.applicationPath + "templates/auction/list_history.html",
                })
                .when("/details/:auctionId", {
                    templateUrl: $window.globalSettings.applicationPath + "templates/auction/details.html",
                    controller: "auction_details_controller",
                }).when("/online/:auctionId", {
                    templateUrl: $window.globalSettings.applicationPath + "templates/auction/online.html",
                    controller: "auction.online",
                });
        });
})();