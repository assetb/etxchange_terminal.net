(function () {
    'use strict';

    angular
        .module('app')
        .directive('fileModel', chose_file_directive);

    chose_file_directive.$inject = ['$window', '$parse'];

    function chose_file_directive($window, $parse) {
        // Usage:
        //     <chose_file></chose_file>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    }

})();