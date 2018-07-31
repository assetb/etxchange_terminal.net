(function () {
    var app = angular.module('app');

    app.controller('auction.online', ['$scope', '$routeParams', '$interval', '$window', 'http_factory',
        function ($scope, $routeParams, $interval, $window, http) {
            $scope.stopInterval = null;
            $scope.urlToApp = $window.globalSettings.urlToApp;
            $scope.$$back = $routeParams.referer;

            $scope.isOnline = false;
            $scope.isConnectToExchange = false;

            $scope.model = {
                refreshTimeOut: 1,
                auctionId: $routeParams.auctionId,
                lotId: null,
                auction: null,
                lot: null,
                priceOffers: null,
                lots: null,
            };

            $scope.updatePriceOffers = function () {
                //http.get($scope.urlToApp + '/api/auction/' + $scope.model.auctionId + '/online', { params: { lotId: $scope.model.lotId } },
                http.get('http://109.120.140.23/ServerApp/api/online/ets', { params: { lots: [$scope.model.lot.Number, ] } },
                    function (data) {
                        $scope.isOnline = true;
                        if (data.code == 200) {
                            $scope.isConnectToExchange = true;
                            $scope.model.priceOffers = data.data;
                        } else {
                            $scope.isConnectToExchange = false;
                            console.error("Error: code", data.code, "description", data.description);
                        }
                    }, function (data, status) {
                        $scope.isOnline = false;
                    });
            };

            $scope.getAuction = function () {
                http.get(baseUrl + "/api/auction/" + $scope.model.auctionId, null,
                    function (data, status, header, config) {
                        $scope.model.auction = data;
                        $scope.model.lot = $scope.model.auction.lots.filter(l => l.Id == $scope.model.lotId)[0];
                    },
                    function (data, status, headers, config) {
                        notification.alert("Ошибка(" + status + ").", "Не удалось загрузить список аукционов.");
                        console.error("GetAuction. Error responce:", status, data);
                        $scope.auction = null;
                    });
            };

            $scope.initInterval = function () {
                $scope.stopInterval = $interval($scope.updatePriceOffers, ($scope.model.refreshTimeOut * 1000));
                $scope.$on('$destroy', function iVeBeenDismissed() {
                    $scope.stopInterval && $interval.cancel($scope.stopInterval);
                });
            };

            $scope.Init = function () {
                $scope.model.lotId = $routeParams.lotId;
                $scope.getAuction();
                $scope.initInterval();

                http.async = true;
            };
        }
    ]);
})();
