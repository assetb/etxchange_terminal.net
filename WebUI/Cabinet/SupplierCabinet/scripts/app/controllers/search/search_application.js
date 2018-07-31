require([
    "angular",
    "scripts/app/controllers/search/list_customers_controller.js",
], function (angular) {
    var app = angular.module("alta.application", [
        'app.search.list',
    ]);
    angular.bootstrap($("#container_application"), ['alta.application']);
})