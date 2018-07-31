require([
    "angular",
    "datepicker",
    "pagination",
    'notification',
    "scripts/app/controllers/auction/auction_details_controller.js",
    "scripts/app/controllers/auction/list_auctions_controller.js",
], function (angular) {
    var app = angular.module("alta.application", [
        'alta.directive.datepicker',
        'alta.directive.pagination',
        'alta.auction.details',
        'alta.auction.list',
        "NotificationModule"
    ]);
    angular.bootstrap($(document), ['alta.application']);
})