/// <reference path="../../../requierjs/require.js" />
/// <reference path="../../../angular.js" />
require([
    "angular",
    "/scripts/app/controllers/report/report.tech.spec.js"
], function (ng) {
    var main = ng.module("altaik.report.main", ["altaik.report.tech.spec"]);

    ng.bootstrap($("#container_application"), ['altaik.report.main']);
});