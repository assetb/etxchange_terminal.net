define([
    "angular",
    "nca"
], function (angular, nca) {
    var module = angular.module("app.services.signDocs", []);

    function dialogDIrective() {
        var directive = {
            template: ""

        };



        return directive;
    }

    function signDocs() {
        this.openDialog = function (defaultDirectory) {

        };
        return this;
    };

    module.factory("signDocs", signDocs);

    return module;
});