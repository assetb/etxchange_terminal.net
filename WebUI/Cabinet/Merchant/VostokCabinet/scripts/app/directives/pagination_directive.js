(function () {
    'use strict';

    angular
        .module('app')
        .directive('pagination', function () {
            return {
                restrict: 'E',
                replace: true,
                template: "<div class=\"medium-12 columns\">" +
                            "<div class=\"medium-2 columns hidden-for-small-down\">" +
                                "<label>Кол-во элементов</label>" +
                                "<ul class=\"pagination\">" +
                                    "<li ng-repeat=\"item in pgListCountItems track by $index\" ng-class=\"{true: 'current', false: ''}[item == pgCountItems]\">" +
                                        "<a ng-click=\"click(pgStartPage, item);\" ng-bind=\"item\"></a>" +
                                    "</li>" +
                                "</ul>" +
                            "</div>" +
                            "<div class=\"medium-7 small-12 columns\">" +
                                "<label class=\"hidden-for-small-down\">Страницы</label>" +
                                "<ul class=\"pagination\">" +
                                    "<li class=\"pagination-previous arrow\">" +
                                        "<a ng-click=\"pgPage != pgStartPage && click(pgStartPage, pgCountItems)\">&laquo;</a>" +
                                    "</li>" +
                                    "<li ng-repeat=\"page in GetPagesForPagination(pgPage, pgCountPages) track by $index\" ng-class=\"{true: 'current', false: ''}[page == pgPage]\">" +
                                        "<a ng-click=\"click(page, pgCountItems)\" ng-bind=\"page\"></a>" +
                                    "</li>" +
                                    "<li class=\"pagination-next arrow\">" +
                                        "<a ng-click=\"pgCountPages != pgPage && click(pgCountPages, pgCountItems)\">&raquo;</a>" +
                                    "</li>" +
                                "</ul>" +
                            "</div>" +
                            "<div class=\"medium-3 columns hidden-for-small-down\">" +
                                "<label ng-bind=\"'Всего найдено: ' + pgCount\"></label>" +
                            "</div>" +
                "</div>",
                controller: ['$scope', function ($scope) {
                    $scope.click = function (page, items) {
                        $scope.pgClick && $scope.pgClick({ page: page, items: items });
                    };
                    $scope.GetPagesForPagination = function (current, count) {
                        var arrayPages = [];
                        var COUNT_PAGINATION_ITEMS = 7;

                        if (current && count) {
                            for (var i = 0; i < COUNT_PAGINATION_ITEMS; i++) {
                                var page;
                                if (current <= Math.floor(COUNT_PAGINATION_ITEMS / 2)) {
                                    page = 1;
                                } else {
                                    if (current >= (count - Math.floor(COUNT_PAGINATION_ITEMS / 2))) {
                                        page = (count - COUNT_PAGINATION_ITEMS + 1);
                                    } else {
                                        page = current - Math.floor(COUNT_PAGINATION_ITEMS / 2);
                                    }
                                }
                                if (page <= 0) page = 1;
                                page = page + i;
                                if (page > count) {
                                    break;
                                }
                                arrayPages.push(page);
                            }
                        }
                        return arrayPages;
                    };
                }],
                scope: {
                    pgCountPages: '=',
                    pgCountItems: '=',
                    pgPage: '=',
                    pgClick: '&',
                    pgCount: '=',

                    pgStartPage: '=?',
                    pgListCountItems: '=?'
                },
                link: function (scope, element, attrs) {
                    if (!scope.pgStartPage) scope.pgStartPage = 1;
                    if (!scope.pgListCountItems) scope.pgListCountItems = [10, 20, 50];
                },
            };
        });
})();