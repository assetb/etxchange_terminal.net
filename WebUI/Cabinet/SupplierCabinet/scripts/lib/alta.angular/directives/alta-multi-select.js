define([
'angular',
'jquery',
'foundation-select'
], function (ng, jquery) {

    var app = ng.module('alta.directive.multiSelect', []);

    app.directive('multiSelect', [function () {
        return {
            restrict: 'E',
            template: "<select multiple id=\"{{guid}}\" data-prompt=\"{{msText ? msText : 'Выберите...'}}\" data-ng-model=\"msValue\" data-ng-options=\"item.id as item.name for item in msOptions track by $index\"/>",
            replace: true,
            transclude: false,
            scope: {
                msText: '@',
                msValue: '=',
                msChanged: '&',
                msOptions: '='
            },
            link:
                function (scope, element, attrs) {
                    scope.guid = (function () {
                        function s4() {
                            return Math.floor((1 + Math.random()) * 0x10000)
                              .toString(16)
                              .substring(1);
                        };
                        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                          s4() + '-' + s4() + s4() + s4();
                    })();

                    var el = jquery(element);
                    el.on('change', function (ev) {
                        scope.$apply(function () {
                            
                        });
                        scope.msChanged && scope.msChanged();
                    })
                }
        }
    }]);
    return app;
});