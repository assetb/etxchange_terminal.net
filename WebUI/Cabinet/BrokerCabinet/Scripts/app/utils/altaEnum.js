define([
    "angular",
    "alta-http"
], function (angular) {
    var app = angular.module("app.utils.AltaEnum", ['app.services.Http']);
    app.factory('enumFactory', ['altaHttp', function (altaHttp) {
        var scope = {
            $url: null,
            $isLoading: false,
            brokerID: null,
            exchangeID: null,
            statusID: null,
            traderID: null,
            $exchanges : {                
                exchangesEnum: [
                    { id: '1', value: 'УТБ Астана филиал в Алматы' },
                    { id: '2', value: 'УТБ Астана' },
                    { id: '4', value: 'Биржа ЕТС' },
                    { id: '5', value: 'КазЭТС' },
                    { id: '3', value: 'Самрук-Казына' },
                ]
            },
            $brokers : {                
                brokersEnum: [
                    { id: '1', value: 'ТОО "Альта и К"' },
                    { id: '2', value: 'ТОО "Корунд-777"' },
                    { id: '3', value: 'ТОО "Альтаир-Нур"' },
                    { id: '4', value: ' ТОО "Ак Алтын Ко"' },
                ]
            },

            $statuses : {                
                statusEnum: [
                    { id: '1', value: 'Новый' },
                    { id: '2', value: 'Состоялся' },
                    { id: '3', value: 'Ожидается' },
                    { id: '4', value: 'Не состоялся' },
                ]
            },

            $traders: {
                tradersEnum: [                    
                    { id: '1', value: 'Не назначен' },
                    { id: '3', value: 'Алтынбек Алия Бакыткызы' },
                    { id: '4', value: 'Азимова Мунира Дильмуратовна' },
                    { id: '5', value: 'Борамбаева Айнагуль Сериковна' },
                    { id: '6', value: 'Нурбосынова Айнур Дастанкызы' },
                    { id: '7', value: 'Михаилова Вероника Сергеевна' },
                    { id: '8', value: 'Танатов Болат Винигарапович' },
                    { id: '9', value: ' Милошенко Игорь Александрович' },                 
                ]
            },

            $auctionTypes: {
                auctionEnum: [
                    { id: '1', value: 'Стандартный биржевой аукцион' },
                    { id: '2', value: 'Двойной встречный аукцион' },
                ]
            },

            rows: [],           
            Search: function GetCustomers() {
                scope.$isLoading = true;
                scope.rows = null;
                altaHttp.get(scope.$url, {
                    method: "GET",
                    //params: scope.params,
                }, function (data, status, header, config) {                    
                    //scope.$countItems = data.countItems;                    
                    //scope.$countPages = data.countPages;
                    scope.success && scope.success(data.rows, status, header, config);
                    scope.rows = data.rows;
                    scope.$isLoading = false;
                });
            }
        };
        return scope;
    }]);
    return app;
    });