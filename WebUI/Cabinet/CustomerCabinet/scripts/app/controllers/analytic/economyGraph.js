require([
 "angular",
 "pagination",
 "datepicker",
 "search",
], function (angular) {
    var app = angular.module("controllers.analytic.EconomyGraph", ["app.services.Search", "alta.directive.datepicker", "alta.directive.pagination"]);
    app.controller("EconomyCrtl", ["$scope", "Search", function ($scope, SearchFactory) {
        SearchFactory.success = function (data) {
            $scope.someF();
        };
        SearchFactory.error = function (responce, status) {
            notification.alert("Ошибка(" + status + ")", "Обтатитесь к брокеру");
        };

        $scope.exchanges = {
            exchangeID: null,
            exchangesEnum: [
             { id: '1', value: 'УТБ Астана филиал в Алматы' },
             { id: '2', value: 'УТБ Астана' },
             { id: '3', value: 'Самрук - Казына' },
             { id: '4', value: 'Биржа ЕТС' },
             { id: '5', value: 'КазЭТС' }
            ]
        };


        $scope.brokers = {
            brokerID: null,
            brokersEnum: [
             { id: '1', value: 'ТОО "Альта и К"' },
             { id: '2', value: 'ТОО "Корунд-777"' },
             { id: '3', value: 'ТОО "Альтаир-Нур"' },
             { id: '4', value: ' ТОО "Ак Алтын Ко"' },
            ]
        };

        $scope.types = {
            typeID: null,
            typesEnum: [
             { id: '1', value: 'Стандартный биржевой аукцион' },
             { id: '2', value: 'Двойной встречный аукцион' },
            ]
        };

        $scope.filterForm = {
            page: 1,
            count: 20
        };


        $scope.number;
        $scope.summ;


        $scope.form = SearchFactory;
        $scope.form.$url = baseUrl + "/api/customer/economy";
        $scope.countItems = $scope.form.$countItems;      
        $scope.isActive = ($scope.form.isActive == 'true');
        //        $scope.form.$url = "http://localhost:8283/api/customer/economy";


        $scope.Init = function () {
            if ($scope.isActive){
            $scope.form.Search();
            $scope.someF();
            } else {
                notification.alert("Ведуться технические работы.", " Обтатитесь к брокеру");
            }
        };


        $scope.search = function (page) {
            if (page) {
                $scope.form.params.page = page;
            } else {
                $scope.form.params.page = 1;
            }

            $scope.form.params.siteId = $scope.exchanges.exchangeID;
            $scope.form.params.brokerId = $scope.brokers.brokerID;
            $scope.form.params.typeId = $scope.types.typeID;
            $scope.form.params.number = $scope.number;
            $scope.form.Search();
            $scope.someF();
        };

        $scope.update = function (page, items) {
            $scope.form.params.countItems = items;
            $scope.search(page);
        };

        $scope.someF = function () {
            $scope.summ = 0;
            angular.forEach($scope.form.rows, function (auction) {
                $scope.summ += +auction.startprice;
            });
        }






    }]);

    angular.bootstrap($("#economyGraph"), ["controllers.analytic.EconomyGraph"]);
});